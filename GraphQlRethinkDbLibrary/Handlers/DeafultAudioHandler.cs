using System;
using System.IO;
using System.Net;
using System.Net.Http.Headers;
using System.Threading.Tasks;
using GraphQL.Conventions;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Http.Features;
using Microsoft.AspNetCore.Mvc;

namespace GraphQlRethinkDbLibrary.Handlers
{
    public class DeafultAudioHandler : SpecialHandler
    {
        public Func<Id, IDefaultAudio> GetAudioFunction { get; }

        public DeafultAudioHandler(Func<Id, IDefaultAudio> getAudioFunction)
        {
            GetAudioFunction = getAudioFunction;
        }

        public override string Path => "/audio/";
        public override Action<HttpContext> Action => context => ReturnObjectById(context, ProcessAudio);

        private async void ProcessAudio(HttpContext context, Id id)
        {
            var audio = GetAudioFunction.Invoke(id);
            context.Response.Headers.Add("Content-Type", audio.ContentType);
            var audioBytes = Convert.FromBase64String(audio.AudioData);
            var length = audioBytes.Length;
            var range = context.Request.Headers["Range"];
            if (!string.IsNullOrEmpty(range.ToString()))
            {
                
            }
            context.Response.Headers.Add("Content-Length", length.ToString());
            context.Response.Headers.Add("Accept-Ranges", "bytes");
            context.Response.Headers.Add("Content-Range", $"bytes 0-{length-1}/{length}");
            context.Response.StatusCode = 200;
            var buffer = 0;
            await context.Response.Body.WriteAsync(audioBytes, 0, audioBytes.Length);
        }
    }

    public interface IDefaultAudio
    {
        string ContentType { get; }
        string AudioData { get; }
    }
}
