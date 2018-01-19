using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using GraphQlRethinkDbLibrary.Schema.Types;

namespace SampleApp.Model
{
    public class FloatTest : NodeBase<FloatTest>
    {
        public double Value { get; }

        public FloatTest(double value)
        {
            Value = value;
        }
    }
}
