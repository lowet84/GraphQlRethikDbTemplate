using System.IO;
using System.Runtime.CompilerServices;
using Microsoft.AspNetCore.Hosting;

namespace GraphQlRethinkDbTemplate
{
    public class Program
    {
        public const string DatabaseName = "RethinkQlSample";

        public static void Main(string[] args)
        {
            var host = new WebHostBuilder()
                .UseKestrel()
                .UseContentRoot(Directory.GetCurrentDirectory())
                .UseIISIntegration()
                .UseStartup<Startup>()
                .UseUrls("http://*:7000")
                .Build();
            //Debug.Init();
            host.Run();
        }
    }
}