using GraphQlRethinkDbTemplate.Attributes;
using GraphQL.Conventions;

namespace GraphQlRethinkDbTemplate.Schema.Types
{
    [Table(nameof(OtherTableChild2)), Description("A child2 class")]
    [UseDefaultDbRead]
    public class OtherTableChild2 : TypeBase<OtherTableChild2>
    {
        public OtherTableChild2(string childText2)
        {
            ChildText2 = childText2;
        }

        public string ChildText2 { get; }
    }
}
