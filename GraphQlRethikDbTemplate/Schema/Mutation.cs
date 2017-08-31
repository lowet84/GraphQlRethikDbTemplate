using GraphQlRethikDbTemplate.Database;
using GraphQlRethikDbTemplate.Schema.Output;
using GraphQlRethikDbTemplate.Schema.Types;
using GraphQL.Conventions;
using GraphQL.Conventions.Relay;

namespace GraphQlRethikDbTemplate.Schema
{
    [ImplementViewer(OperationType.Mutation)]
    public class Mutation
    {
        [Description("Add a new test item")]
        public DefaultResult<Test> AddTest(
            [Inject] IDbContext context,
            string title)
        {
            var newTest = new Test(title);
            var ret = DbContext.Instance.AddDefault(newTest);
            return new DefaultResult<Test>(ret);
        }
    }
}
