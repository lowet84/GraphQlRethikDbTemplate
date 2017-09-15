using System;
using System.Threading.Tasks;
using GraphQL.Conventions;
using Microsoft.AspNetCore.Http;

namespace GraphQlRethinkDbLibrary.Handlers
{
    public abstract class DefaultImageHandler : SpecialHandler
    {
        public abstract IDefaultImage GetImage(Id id);

        public override string Path => "/images/";
        public override async Task Process(HttpContext context)
        {
            try
            {
                if (string.Compare(context.Request.Method, "GET", StringComparison.OrdinalIgnoreCase) == 0)
                {
                    var idString = context.Request.Path.Value.Substring(Path.Length);
                    var id = new Id(idString);
                    var image = GetImage(id);
                    context.Response.Headers.Add("Content-Type", image.ContentType);
                    var imageBytes = image.ImageData;
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

    public interface IDefaultImage
    {
        string ContentType { get; }
        [Ignore]
        byte[] ImageData { get; }
    }
}
