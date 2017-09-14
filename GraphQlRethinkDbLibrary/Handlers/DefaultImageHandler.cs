using System;
using GraphQL.Conventions;
using Microsoft.AspNetCore.Http;

namespace GraphQlRethinkDbLibrary.Handlers
{
    public class DefaultImageHandler : SpecialHandler
    {
        public Func<Id, IDefaultImage> GetImageFunction { get; }

        public DefaultImageHandler(Func<Id, IDefaultImage> getImageFunction)
        {
            GetImageFunction = getImageFunction;
        }

        public override string Path => "/images/";
        public override void Process(HttpContext context)
        {
            try
            {
                if (string.Compare(context.Request.Method, "GET", StringComparison.OrdinalIgnoreCase) == 0)
                {
                    var idString = context.Request.Path.Value.Substring(Path.Length);
                    var id = new Id(idString);
                    var image = GetImageFunction.Invoke(id);
                    context.Response.Headers.Add("Content-Type", image.ContentType);
                    var imageBytes = image.ImageData;
                    context.Response.StatusCode = 200;
                    context.Response.Body.WriteAsync(imageBytes, 0, imageBytes.Length).Wait();
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

    public interface IDefaultImage
    {
        string ContentType { get; }
        [Ignore]
        byte[] ImageData { get; }
    }
}
