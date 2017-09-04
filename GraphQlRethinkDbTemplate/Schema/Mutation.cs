using System.Collections.Generic;
using GraphQlRethinkDbTemplate.Schema.Output;
using GraphQlRethinkDbTemplate.Schema.Types;
using GraphQL.Conventions;
using GraphQL.Conventions.Relay;
using OperationType = GraphQL.Conventions.Relay.OperationType;

namespace GraphQlRethinkDbTemplate.Schema
{
    [ImplementViewer(OperationType.Mutation)]
    public class Mutation
    {
        [Description("Add a new test item")]
        public DefaultResult<Test> AddTest(
            UserContext context,
            string title)
        {
            var newTest = new Test(title, null);
            var ret = context.AddDefault(newTest);
            return new DefaultResult<Test>(ret);
        }

        [Description("Add child to test")]
        public DefaultResult<Test> AddChildToTest(
            UserContext context,
            Id testId,
            Id childId)
        {
            var oldTest = context.Get<Test>(testId, UserContext.ReadType.Shallow);
            //var query = $"query{{child(id:\"{childId}\"){{id}}}}";
            //var document = UserContext.GetDocument(query);
            var child = Utils.CreateDummyObject<OtherTableChild>(childId); //context.Get<OtherTableChild>(childId, document);
            var newTest = new Test(oldTest.Text, oldTest.OtherTableChildren.AddOrInitializeArray(child));
            var ret = context.AddDefault(newTest);
            return new DefaultResult<Test>(ret);
        }

        [Description("Add new child")]
        public DefaultResult<OtherTableChild> AddChild(
            UserContext context,
            string text)
        {
            var newChild = new OtherTableChild(text);
            var ret = context.AddDefault(newChild);
            return new DefaultResult<OtherTableChild>(ret);
        }
    }
}
