using System.Threading.Tasks;
using GraphQlRethinkDbTemplate.Schema.Types;
using GraphQL.Conventions;
using GraphQL.Conventions.Relay;

namespace GraphQlRethinkDbTemplate.Schema
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

        [Description("Get child by id")]
        public Task<OtherTableChild> Child(UserContext context, Id id)
        {
            var data = context.Get<OtherTableChild>(id);
            return Task.FromResult(data);
        }

        public Task<Test> Shallow(UserContext context, Id id)
        {
            var data = context.Get<Test>(id, UserContext.ReadType.Shallow);
            return Task.FromResult(data);
        }
    }
}
