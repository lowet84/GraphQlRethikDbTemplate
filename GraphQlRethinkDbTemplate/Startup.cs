using System;
using GraphQlRethinkDbLibrary;
using GraphQlRethinkDbLibrary.Handlers;
using GraphQlRethinkDbTemplate.Model;
using GraphQlRethinkDbTemplate.Schema;
using GraphQL.Conventions;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Logging;
using Query = GraphQlRethinkDbTemplate.Schema.Query;

namespace GraphQlRethinkDbTemplate
{
    public class Startup
    {
        public void Configure(IApplicationBuilder app, IHostingEnvironment env, ILoggerFactory loggerFactory)
        {
            loggerFactory.AddConsole();
            var handler =
                GraphQlRethinkDbHandler<Query, Mutation>.Create("localhost", "GraphQlRethinkDbTemplate",
                new DefaultImageHandler(GetImageString));
            app.Run(handler.DeafultHandleRequest);
        }

        private static Image GetImageString(Id id)
        {
            if (!id.IsIdentifierForType<Image>())
                throw new Exception("Id is not valid for image type");
            var image = new UserContext(null).Get<Image>(id, UserContext.ReadType.Shallow);
            return image;
        }
    }
}