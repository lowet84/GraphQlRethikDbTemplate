using System;
using GraphQL.Conventions;
using Microsoft.AspNetCore.Http;

namespace GraphQlRethinkDbLibrary.Handlers
{
    public class DefaultImageHandler : SpecialHandler
    {
        public Func<Id, IDeafultImage> GetImageFunction { get; }

        public DefaultImageHandler(Func<Id, IDeafultImage> getImageFunction)
        {
            GetImageFunction = getImageFunction;
        }

        public override string Path => "/images/";
        public override Action<HttpContext> Action => context => ReturnObjectById(context, ProcessImage);

        private async void ProcessImage(HttpContext context, Id id)
        {
            var image = GetImageFunction.Invoke(id);
            context.Response.Headers.Add("Content-Type", image.ContentType);
            var imageBytes = Convert.FromBase64String(image.ImageData);
            context.Response.StatusCode = 200;
            await context.Response.Body.WriteAsync(imageBytes, 0, imageBytes.Length);
        }
    }

    public interface IDeafultImage
    {
        string ContentType { get; }
        string ImageData { get; }
    }
}
