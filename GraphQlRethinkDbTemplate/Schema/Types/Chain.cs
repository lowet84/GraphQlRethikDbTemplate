using GraphQlRethinkDbTemplate.Attributes;
using GraphQL.Conventions;

namespace GraphQlRethinkDbTemplate.Schema.Types
{
    [Description("The chain")]
    public class Chain : NodeBase<Chain>
    {
        public Chain(Id currentId, Id[] history)
        {
            CurrentId = currentId;
            History = history;
        }

        public Id CurrentId { get; }

        public Id[] History { get; }
    }
}
