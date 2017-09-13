using System;
using System.Threading.Tasks;
using GraphQL.Conventions;
using Microsoft.AspNetCore.Http;

namespace GraphQlRethinkDbLibrary.Handlers
{
    public abstract class SpecialHandler
    {
        public abstract string Path { get; }
        public abstract void Process(HttpContext context);
    }
}
