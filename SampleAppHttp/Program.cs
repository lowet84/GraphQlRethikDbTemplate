using System;
using GraphQlRethinkDbHttp;

namespace SampleAppHttp
{
    class Program
    {
        static void Main(string[] args)
        {
            var server = new SimpleHttpServer(3000, "localhost");
        }
    }
}
