using System.IO;
using GraphQlRethinkDbTemplate.Database;
using Microsoft.AspNetCore.Hosting;

namespace GraphQlRethinkDbTemplate
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var dbContext = DbContext.Instance;
            var host = new WebHostBuilder()
                .UseKestrel()
                .UseContentRoot(Directory.GetCurrentDirectory())
                .UseIISIntegration()
                .UseStartup<Startup>()
                .UseUrls("http://*:7000")
                .Build();

            host.Run();
        }
    }
}