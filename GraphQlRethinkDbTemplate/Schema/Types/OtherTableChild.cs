using GraphQlRethinkDbTemplate.Attributes;
using GraphQL.Conventions;
using Newtonsoft.Json;

namespace GraphQlRethinkDbTemplate.Schema.Types
{
    [Table(nameof(OtherTableChild)), Description("A child class")]
    [UseDefaultDbRead]
    public class OtherTableChild : TypeBase<OtherTableChild>
    {
        public OtherTableChild(string childText, OtherTableChild2[] otherTableChildren2)
        {
            ChildText = childText;
            OtherTableChildren2 = otherTableChildren2;
        }

        public string ChildText { get; }

        [Description("Items in another table2")]
        public OtherTableChild2[] OtherTableChildren2 { get; }
    }
}
