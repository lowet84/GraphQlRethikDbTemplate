using GraphQlRethinkDbTemplate.Attributes;
using GraphQL.Conventions;

namespace GraphQlRethinkDbTemplate.Schema.Types
{
    [Table(nameof(OtherTableChild)), Description("A child class")]
    [UseDefaultDbRead]
    public class OtherTableChild : TypeBase<OtherTableChild>
    {
        public OtherTableChild(string childText)
        {
            ChildText = childText;
        }

        public string ChildText { get; }
    }
}
