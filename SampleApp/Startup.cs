using GraphQlRethinkDbLibrary.Database;
using GraphQlRethinkDbLibrary.Handlers;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Logging;
using SampleApp.Handlers;
using SampleApp.Schema;

namespace SampleApp
{
    public class Startup
    {
        public void Configure(IApplicationBuilder app, IHostingEnvironment env, ILoggerFactory loggerFactory)
        {
            BaseHandler.Setup(
                app,
                env,
                new DatabaseName(Program.DatabaseName),
                new DatabaseUrl("localhost"),
                new GraphQlDefaultHandler<Query,Mutation>(),
                new ImageFileHandler(),
                new AudioFileHandler());
        }
    }
}