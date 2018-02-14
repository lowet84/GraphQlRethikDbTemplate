using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using GraphQlRethinkDbCore.Schema.Types;

namespace SampleApp.Model
{
    public class DateTimeTest : NodeBase<DateTimeTest>
    {
        public DateTime Value { get; }

        public DateTimeTest(DateTime value)
        {
            Value = value;
        }
    }
}
