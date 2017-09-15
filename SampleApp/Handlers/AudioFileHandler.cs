using System;
using System.IO;
using System.Linq;
using GraphQlRethinkDbLibrary.Handlers;
using GraphQL.Conventions;
using SampleApp.Model;

namespace SampleApp.Handlers
{
    public class AudioFileHandler : DefaultAudioHandler
    {
        public override IDefaultAudio GetAudio(string key)
        {
            var id = new Id(key);
            if (id.IsIdentifierForType<Audio>())
            {
                return HandlerUtil.Get<Audio>(id);
            }
            if (id.IsIdentifierForType<AudioFile>())
            {
                return HandlerUtil.Get<AudioFile>(id);
            }
            throw new NotImplementedException();
        }

        public override byte[] GetData(string key, int part)
        {
            var id = new Id(key);
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
            var audio = HandlerUtil.Get<AudioFile>(id);
            var fileStream = File.OpenRead(audio.FileName);
            fileStream.Seek(part * audio.BlockSize, SeekOrigin.Begin);
            var buffer = new byte[audio.BlockSize];
            var readBytes = fileStream.Read(buffer, 0, buffer.Length);
            return buffer.Take(readBytes).ToArray();
        }

        private byte[] GetDataFromDb(Id id, int part)
        {
            var audio = HandlerUtil.Get<Audio>(id);
            var data = HandlerUtil.Get<Audio.AudioData>(audio.AudioDataParts[part].Id);
            return Convert.FromBase64String(data.Data);
        }
    }
}