using System;
using GraphQL.Conventions;
using Microsoft.AspNetCore.Http;

namespace GraphQlRethinkDbLibrary.Handlers
{
    public class DefaultImageHandler : SpecialHandler
    {
        public Func<Id, IDeafultImage> GetByte64EncodedImageFunction { get; }

        public DefaultImageHandler(Func<Id, IDeafultImage> getByte64EncodedImageFunction)
        {
            GetByte64EncodedImageFunction = getByte64EncodedImageFunction;
        }

        public override string Path => "/images/";
        public override Action<HttpContext> Action => ReturnImage;

        private async void ReturnImage(HttpContext context)
        {
            try
            {
                if (string.Compare(context.Request.Method, "GET", StringComparison.OrdinalIgnoreCase) == 0)
                {
                    var idString = context.Request.Path.Value.Substring("/images/".Length);
                    var id = new Id(idString);
                    var image = GetByte64EncodedImageFunction.Invoke(id);
                    context.Response.Headers.Add("Content-Type", image.ContentType);
                    var imageBytes = Convert.FromBase64String(image.ImageData);
                    context.Response.StatusCode = 200;
                    await context.Response.Body.WriteAsync(imageBytes, 0, imageBytes.Length);
                    return;
                }
            }
            catch (Exception)
            {
                // ignored
            }

            context.Response.StatusCode = 400;
        }
    }

    public interface IDeafultImage
    {
        string ContentType { get; }
        string ImageData { get; }
    }
}
