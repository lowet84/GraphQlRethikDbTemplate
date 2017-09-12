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
        private readonly GraphQlRethinkDbHandler<Query, Mutation> _handler = new GraphQlRethinkDbHandler<Query, Mutation>("localhost");

        public void ConfigureServices(IServiceCollection services)
        {
        }

        public void Configure(IApplicationBuilder app, IHostingEnvironment env, ILoggerFactory loggerFactory)
        {
            loggerFactory.AddConsole();

            app.Run(_handler.DeafultHandleRequest);
        }
    }
}