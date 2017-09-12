using System;
using System.IO;
using System.Linq;
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
            //Init();
            host.Run();
        }

        public static void Init()
        {
            var author = new Author("Axel", "Axelsson");
            var author2 = new Author("Bengt", "Bengtsson");
            var book = new Book("En bok", author);
            var series = new Series("En serie böcker", null);
            var series2 = new Series("En serie böcker till", null);
            var newSeries = new Series(series.Name, new[] { book });

            var query =
                @"query{series(id:""#####""){authors{name{fistName, lastName}} name books{id title bookAuthors{author{name{fistName lastName}}}} }}";
            query = query.Replace("#####", series.Id.ToString());
            var hostName = Environment.GetEnvironmentVariable("DATABASE");
            var userContext = new UserContext(query, hostName);
            userContext.Reset();

            userContext.AddDefault(author);
            userContext.UpdateDefault(author2, author.Id);
            userContext.AddDefault(book);
            userContext.AddDefault(series);
            userContext.AddDefault(series2);
            userContext.UpdateDefault(newSeries, series.Id);

            var readSeries = userContext.Get<Series>(series.Id);

            var searchObject = new SearchObject<Series>()
                .Add(SearchOperationType.AnyEquals, nameof(Series.Books), readSeries.Books.First().Id.ToString());
            var results = userContext.Search(searchObject);
        }
    }
}