﻿using System;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;
using GraphQlRethinkDbLibrary;
using GraphQlRethinkDbLibrary.Handlers;
using GraphQlRethinkDbLibrary.Schema;
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
            var handler =
                GraphQlRethinkDbHandler<Query, Mutation>.Create("localhost", "GraphQlRethinkDbTemplate",
                new DefaultImageHandler(Get<Image>),
                new DeafultAudioHandler(Get<AudioFile>, GetAudioFileData));
            app.Run(handler.DeafultHandleRequest);
        }

        private static T Get<T>(Id id) where T: class
        {
            var type = typeof(T);
            if (!id.IsIdentifierForType<T>())
                throw new Exception($"Id is not valid for type {type.Name}");
            var item = new UserContext(null).Get<T>(id, UserContext.ReadType.Shallow);
            return item;
        }

        private static byte[] GetAudioData(Id id, int part)
        {
            if (!id.IsIdentifierForType<Audio>())
                throw new Exception("Id is not valid for type AudioData");
            var audio = Get<Audio>(id);
            var data = Get<AudioData>(audio.AudioData[part].Id);
            return Convert.FromBase64String(data.Data);
        }

        private static byte[] GetAudioFileData(Id id, int part)
        {
            if (!id.IsIdentifierForType<AudioFile>())
                throw new Exception("Id is not valid for type AudioData");
            var audio = Get<AudioFile>(id);
            var fileStream = File.OpenRead(audio.FileName);
            fileStream.Seek(part * audio.BlockSize, SeekOrigin.Begin);
            var buffer = new byte[audio.BlockSize];
            var readBytes = fileStream.Read(buffer, 0, buffer.Length);
            return buffer.Take(readBytes).ToArray();
        }
    }
}