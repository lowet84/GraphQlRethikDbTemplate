using System.Linq;
using System.Threading.Tasks;
using GraphQlRethinkDbLibrary.Database;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.StaticFiles;

namespace GraphQlRethinkDbLibrary.Handlers
{
    public class BaseHandler
    {
        private static SpecialHandler[] SpecialHandlers { get; set; }
        private static IHostingEnvironment Env { get; set; }

        public static void Setup(
            IApplicationBuilder app,
            IHostingEnvironment env,
            DatabaseName databaseName,
            DatabaseUrl databaseUrl,
            params SpecialHandler[] specialHandlers)
        {
            new UserContext(null, databaseUrl, databaseName);
            SpecialHandlers = specialHandlers;
            Env = env;
            app.Run(DefaultHandleRequest);
        }

        private static async Task DefaultHandleRequest(HttpContext context)
        {
            var specialHandler = SpecialHandlers
                    .FirstOrDefault(d => context.Request.Path.Value.StartsWith(d.Path));

            if (specialHandler != null)
                await specialHandler.Process(context);
            else
            {
                await ProcessStaticFiles(context);
            }
        }

        public static async Task ProcessStaticFiles(HttpContext context)
        {
            const string indexDefault = "/index.html";
            var path = context.Request.Path;
            if (path == "/") path = indexDefault;
            var provider = Env.WebRootFileProvider;
            var fileInfo = provider.GetFileInfo(path);
            new FileExtensionContentTypeProvider().TryGetContentType(fileInfo.Name, out var contentType);
            context.Response.Headers.Add("Content-Type", contentType);
            await context.Response.SendFileAsync(fileInfo);
        }
    }
}
