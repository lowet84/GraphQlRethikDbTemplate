using System;
using System.Collections.Generic;
using System.Net;

namespace GraphQlRethinkDbHttp.Handlers
{
    public abstract class SpecialHandler
    {
        public abstract string Path { get; }
        public abstract void Process(HttpListenerContext context);
    }
}
