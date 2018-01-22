using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using GraphQlRethinkDbLibrary.Handlers;
using GraphQLParser.AST;
using GraphQlRethinkDbLibrary;
using Microsoft.AspNetCore.Http;

namespace SampleApp.Handlers
{
    public class GraphQlHandler : GraphQlDefaultHandler
    {
        public override void HandleError(string errorMessage)
        {
            base.HandleError(errorMessage);
        }
    }
}
