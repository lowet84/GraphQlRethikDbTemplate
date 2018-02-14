using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using GraphQlRethinkDbCore.Schema.Types;

namespace SampleApp.Model
{
    public class BoolTest: NodeBase<BoolTest>
    {
        public bool Value { get; }

        public BoolTest(bool value)
        {
            Value = value;
        }
    }
}
