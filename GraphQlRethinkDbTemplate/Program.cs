using System;
using System.IO;
using System.Linq;
using System.Net.Http;
using GraphQlRethinkDbLibrary;
using GraphQlRethinkDbLibrary.Database.Search;
using GraphQlRethinkDbTemplate.Model;
using Microsoft.AspNetCore.Hosting;

namespace GraphQlRethinkDbTemplate
{
    public class Program
    {
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