using System;
using GraphQL.Conventions;
using Microsoft.AspNetCore.Http;

namespace GraphQlRethinkDbLibrary.Handlers
{
    public abstract class SpecialHandler
    {
        public abstract string Path { get; }
        public abstract Action<HttpContext> Action { get; }

        protected async void ReturnObjectById(HttpContext context, Action<HttpContext, Id> innerAction)
        {
            try
            {
                if (string.Compare(context.Request.Method, "GET", StringComparison.OrdinalIgnoreCase) == 0)
                {
                    var idString = context.Request.Path.Value.Substring(Path.Length);
                    var id = new Id(idString);
                    innerAction.Invoke(context, id);
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
}
