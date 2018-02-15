using System;
using System.Collections.Generic;
using System.Text;
using GraphQlRethinkDbCore;
using GraphQlRethinkDbCore.Schema.Output;
using GraphQL.Conventions.Relay;

namespace SampleAppHttp.Schema
{
    [ImplementViewer(OperationType.Query)]
    public class Query
    {
        public DefaultResult<bool> Test(UserContext context)
        {
            return new DefaultResult<bool>(true);
        }
    }
}
