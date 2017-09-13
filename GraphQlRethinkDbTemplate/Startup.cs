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
                new DefaultImageHandler(Get<Image>),
                new DeafultAudioHandler(Get<Audio>, Get<AudioData>));
            app.Run(handler.DeafultHandleRequest);
        }

        private static T Get<T>(Id id) where T: class
        {
            var type = typeof(T);
            if (!id.IsIdentifierForType<T>())
                throw new Exception($"Id is not valid for type {type.Name}");
            var item = new UserContext(null).Get<T>(id, UserContext.ReadType.Shallow);
            return item;
        }
    }
}