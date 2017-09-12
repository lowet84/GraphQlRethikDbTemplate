using GraphQlRethinkDbLibrary;
using GraphQlRethinkDbTemplate.Schema;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;

namespace GraphQlRethinkDbTemplate
{
    public class Startup
    {
        public void Configure(IApplicationBuilder app, IHostingEnvironment env, ILoggerFactory loggerFactory)
        {
            loggerFactory.AddConsole();
            var handler =
                GraphQlRethinkDbHandler<Query, Mutation>.Create("localhost");
            app.Run(handler.DeafultHandleRequest);
        }
    }
}