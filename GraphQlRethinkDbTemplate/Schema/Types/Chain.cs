using GraphQlRethinkDbTemplate.Attributes;
using GraphQL.Conventions;

namespace GraphQlRethinkDbTemplate.Schema.Types
{
    [Table(nameof(Chain)), Description("The chain")]
    [UseDefaultDbRead]
    public class Chain : TypeBase<Chain>
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
