﻿using GraphQlRethikDbTemplate.Attributes;
using GraphQL.Conventions;

namespace GraphQlRethikDbTemplate.Schema.Types
{
    [UseDeafultDbRead, Table(nameof(Test)), Description("A test class"),]
    public class Test : TypeBase<Test>
    {
        public Test(string text)
        {
            Text = text;
        }

        [Description("A test class")]
        public string Text { get; }
    }
}
