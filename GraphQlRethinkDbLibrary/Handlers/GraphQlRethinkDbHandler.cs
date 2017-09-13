using System;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;
using GraphQlRethinkDbLibrary.Schema;
using GraphQL.Conventions.Relay;
using GraphQL.Conventions.Web;
using Microsoft.AspNetCore.Http;

namespace GraphQlRethinkDbLibrary.Handlers
{
    public interface IGraphQlRethinkDbHandler
    {
        Task DeafultHandleRequest(HttpContext context);
    }

    public class GraphQlRethinkDbHandler<TQuery, TMutation> : IGraphQlRethinkDbHandler
    {
        public SpecialHandler[] SpecialHandlers { get; }
        private readonly IRequestHandler _requestHandler;

        public static IGraphQlRethinkDbHandler Create(string databaseHost, string databaseName, params SpecialHandler[] specialHandlers)
        {
            return new GraphQlRethinkDbHandler<TQuery, TMutation>(databaseHost, databaseName, specialHandlers);
        }

        private GraphQlRethinkDbHandler(string databaseHost, string databaseName, params SpecialHandler[] specialHandlers)
        {
            SpecialHandlers = specialHandlers;
            var queryType = typeof(TQuery);
            var mutationType = typeof(TMutation);

            var queryAttribute = queryType.GetCustomAttribute<ImplementViewerAttribute>();
            var mutationAttribute = mutationType.GetCustomAttribute<ImplementViewerAttribute>();
            if (!MatchesOperationType(queryAttribute, OperationType.Query))
            {
                throw new Exception("Query must have attribute [ImplementViewer(OperationType.Query)]");
            }
            if (!MatchesOperationType(mutationAttribute, OperationType.Mutation))
            {
                throw new Exception("Mutation must have attribute [ImplementViewer(OperationType.Mutation)]");
            }

            new UserContext(null, databaseHost, databaseName);
            _requestHandler = RequestHandler
                .New()
                .WithQueryAndMutation<TQuery, TMutation>()
                .Generate();
        }

        public async Task DeafultHandleRequest(HttpContext context)
        {
            var specialHandler = SpecialHandlers.FirstOrDefault(d => context.Request.Path.Value.StartsWith(d.Path));
            if (specialHandler != null)
            {
                specialHandler.Process(context);
                return;
            }

            if (string.Compare(context.Request.Method, "OPTIONS", StringComparison.OrdinalIgnoreCase) == 0)
            {
                context.Response.StatusCode = 200;
                return;
            }

            if (string.Compare(context.Request.Method, "POST", StringComparison.OrdinalIgnoreCase) != 0)
            {
                context.Response.StatusCode = 400;
                return;
            }

            var streamReader = new StreamReader(context.Request.Body);
            var body = streamReader.ReadToEnd();
            var userContext = new UserContext(body);
            var result = await _requestHandler
                .ProcessRequest(Request.New(body), userContext);
            context.Response.Headers.Add("Content-Type", "application/json; charset=utf-8");
            context.Response.StatusCode = result.Errors?.Count > 0 ? 400 : 200;
            await context.Response.WriteAsync(result.Body);
        }

        private bool MatchesOperationType(ImplementViewerAttribute attribute, OperationType operationType)
        {
            var attributeOperationType = Utils.GetFields(typeof(ImplementViewerAttribute)).First(d => d.Name == "_operationType")
                .GetValue(attribute) as OperationType?;
            return attributeOperationType == operationType;
        }
    }
}
