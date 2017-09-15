using System;
using System.IO;
using System.Linq;
using GraphQlRethinkDbLibrary;
using GraphQlRethinkDbLibrary.Database;
using GraphQlRethinkDbLibrary.Handlers;
using GraphQlRethinkDbTemplate.Model;
using GraphQlRethinkDbTemplate.Schema;
using GraphQL.Conventions;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Logging;
using Query = GraphQlRethinkDbTemplate.Schema.Query;

namespace GraphQlRethinkDbTemplate
{
    public class Startup
    {
        public void Configure(IApplicationBuilder app, IHostingEnvironment env, ILoggerFactory loggerFactory)
        {
            loggerFactory.AddConsole();
            BaseHandler.Setup(
                app,
                env,
                new DatabaseName(Program.DatabaseName), 
                new DatabaseUrl("localhost"),
                new GraphQlHandler(),
                new ImageFileHandler(),
                new AudioFileHandler());
        }

        public class ImageFileHandler : DefaultImageHandler
        {
            public override IDefaultImage GetImage(Id id)
            {
                if (id.IsIdentifierForType<Image>())
                {
                    return Get<Image>(id);
                }
                if (id.IsIdentifierForType<ImageFile>())
                {
                    return GetImageFromFile(id);
                }
                throw new NotImplementedException();
            }

            private IDefaultImage GetImageFromFile(Id id)
            {
                var imageFile = Get<ImageFile>(id);
                var bytes = File.ReadAllBytes(imageFile.FileName);
                var ret = new ImageFile.ImageFileData("image/jpeg", bytes);
                return ret;
            }
        }

        public class GraphQlHandler : GraphQlDefaultHandler<Query, Mutation> { }

        public class AudioFileHandler : DefaultAudioHandler
        {
            public override IDefaultAudio GetAudio(Id id)
            {
                if (id.IsIdentifierForType<Audio>())
                {
                    return Get<Audio>(id);
                }
                if (id.IsIdentifierForType<AudioFile>())
                {
                    return Get<AudioFile>(id);
                }
                throw new NotImplementedException();
            }

            public override byte[] GetData(Id id, int part)
            {
                if (id.IsIdentifierForType<Audio>())
                {
                    return GetDataFromDb(id, part);
                }
                if (id.IsIdentifierForType<AudioFile>())
                {
                    return GetDataFromFile(id, part);
                }
                throw new NotImplementedException();
            }

            private byte[] GetDataFromFile(Id id, int part)
            {
                var audio = Get<AudioFile>(id);
                var fileStream = File.OpenRead(audio.FileName);
                fileStream.Seek(part * audio.BlockSize, SeekOrigin.Begin);
                var buffer = new byte[audio.BlockSize];
                var readBytes = fileStream.Read(buffer, 0, buffer.Length);
                return buffer.Take(readBytes).ToArray();
            }

            private byte[] GetDataFromDb(Id id, int part)
            {
                var audio = Get<Audio>(id);
                var data = Get<Audio.AudioData>(audio.AudioDataParts[part].Id);
                return Convert.FromBase64String(data.Data);
            }
        }

        private static T Get<T>(Id id) where T : class
        {
            var type = typeof(T);
            if (!id.IsIdentifierForType<T>())
                throw new Exception($"Id is not valid for type {type.Name}");
            var item = new UserContext().Get<T>(id, UserContext.ReadType.Shallow);
            return item;
        }
    }
}