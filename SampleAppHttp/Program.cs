using System;
using GraphQlRethinkDbCore.Database;
using GraphQlRethinkDbHttp;
using GraphQlRethinkDbHttp.Handlers;
using SampleAppHttp.Handlers;

namespace SampleAppHttp
{
    public class Program
    {
        public const string DatabaseName = "RethinkQlSampleHttp";

        static void Main(string[] args)
        {
            var server = new SimpleHttpServer(
                3000,
                "localhost",
                new DatabaseName(DatabaseName),
                new DatabaseUrl(Environment.GetEnvironmentVariable("DATABASE")),
                new GraphQlDefaultHandler(),
                new ImageFileHandler()
            );
        }
    }
}
