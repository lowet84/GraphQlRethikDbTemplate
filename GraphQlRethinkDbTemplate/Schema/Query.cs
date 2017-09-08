using System.Threading.Tasks;
using GraphQlRethinkDbTemplate.Schema.Model;
using GraphQlRethinkDbTemplate.Schema.Types;
using GraphQL.Conventions;
using GraphQL.Conventions.Relay;

namespace GraphQlRethinkDbTemplate.Schema
{
    [ImplementViewer(OperationType.Query)]
    public class Query
    {
        [Description("Get author by id")]
        public Task<Author> Author(UserContext context, Id id)
        {
            var data = context.Get<Author>(id);
            return Task.FromResult(data);
        }

        [Description("Get book by id")]
        public Task<Book> Book(UserContext context, Id id)
        {
            var data = context.Get<Book>(id);
            return Task.FromResult(data);
        }

        [Description("Get a series of books")]
        public Task<Series> Series(UserContext context, Id id)
        {
            var data = context.Get<Series>(id);
            return Task.FromResult(data);
        }
    }
}
