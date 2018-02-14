using GraphQlRethinkDbCore.Database;
using GraphQlRethinkDbLibrary.Handlers;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;

using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using SampleApp.Handlers;
using SampleApp.Schema;

namespace SampleApp
{
    public class Startup
    {
        public void Configure(IApplicationBuilder app, IHostingEnvironment env, ILoggerFactory loggerFactory)
        {
            app.UseCors(builder =>
            {
                builder.AllowAnyOrigin().AllowAnyMethod().AllowAnyHeader();
            });

            BaseHandler.Setup(
                app,
                env,
                new DatabaseName(Program.DatabaseName),
                new DatabaseUrl("localhost"),
                new GraphQlHandler(),
                new ImageFileHandler(),
                new AudioFileHandler());
        }

        public void ConfigureServices(IServiceCollection services)
        {
            // Add framework services.
            services.AddCors();
        }
    }
}