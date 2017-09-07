using GraphQlRethinkDbTemplate.Attributes;
using GraphQL.Conventions;

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
        public OtherTableChild[] OtherTableChildren { get; }
    }
}
