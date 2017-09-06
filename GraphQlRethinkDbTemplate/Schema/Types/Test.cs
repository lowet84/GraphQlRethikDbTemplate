using GraphQlRethinkDbTemplate.Attributes;
using GraphQlRethinkDbTemplate.Schema.Types.Converters;
using GraphQL.Conventions;
using Newtonsoft.Json;

namespace GraphQlRethinkDbTemplate.Schema.Types
{
    [UseDefaultDbRead]
    [Table(nameof(Test)), Description("A test class")]
    public class Test : TypeBase<Test>
    {
        public Test(string text, OtherTableChild[] otherTableChildren)
        {
            Text = text;
            OtherTableChildren = otherTableChildren;
        }

        [Description("A test class")]
        public string Text { get; }

        [Description("Items in another table")]
        [JsonConverter(typeof(OtherTableConverter))]
        public OtherTableChild[] OtherTableChildren { get; }
    }
}
