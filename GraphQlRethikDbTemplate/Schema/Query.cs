using System.Threading.Tasks;
using GraphQlRethikDbTemplate.Schema.Types;
using GraphQL.Conventions;
using GraphQL.Conventions.Relay;

namespace GraphQlRethikDbTemplate.Schema
{
    [ImplementViewer(OperationType.Query)]
    public class Query
    {
        [Description("Get node by id")]
        public Task<Test> Test(UserContext context, Id id)
        {
            var data = context.Get<Test>(id);
            return Task.FromResult(data);
        }
    }
}
