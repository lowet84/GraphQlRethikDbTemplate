using System;
using Microsoft.AspNetCore.Http;

namespace GraphQlRethinkDbLibrary.Handlers
{
    public abstract class SpecialHandler
    {
        public abstract string Path { get; }
        public abstract Action<HttpContext> Action { get; }
    }
}
