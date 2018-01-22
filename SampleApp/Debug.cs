using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using GraphQlRethinkDbLibrary;
using GraphQlRethinkDbLibrary.Database;
using RethinkDb.Driver.Ast;
using SampleApp.Model;
using Random = System.Random;

namespace SampleApp
{
    public static class Debug
    {
        public static void Run()
        {
            Reset();
            //Delete();
            //Basic();
            //AudioAndImage();
            //Clean();
            //FixIssues();
            //NullProperty();
            //Boolean();
            //DateTime();
            //Float();
        }

        private static void FixIssues()
        {
            var searches = new[] {"serie", "Andra"};
            var result =
                UserContext.SearchShallow<Series>(
                    d => d.Filter(series => series.G("Name").Match(string.Join("|", searches))));
        }

        private static void Basic()
        {
            var author = new Author("Axel", "Axelsson");
            var author2 = new Author("Bengt", "Bengtsson");
            var book = new Book("En bok", author);
            var series = new Series("En serie böcker", null);
            var series2 = new Series("Andra böcker", null);
            var newSeries = new Series(series.Name, new[] { book });

            var query =
                @"query{series(id:""#####""){authors{name{fistName, lastName}} name books{id title bookAuthors{author{name{fistName lastName}}}} }}";
            query = query.Replace("#####", series.Id.ToString());
            var hostName = Environment.GetEnvironmentVariable("DATABASE");
            var userContext = new UserContext(query, new DatabaseUrl(hostName), new DatabaseName(Program.DatabaseName));

            UserContext.AddDefault(author);
            UserContext.UpdateDefault(author2, author.Id);
            UserContext.AddDefault(book);
            UserContext.AddDefault(series);
            UserContext.AddDefault(series2);
            UserContext.UpdateDefault(newSeries, series.Id);

            var readSeries = userContext.Get<Series>(series.Id);

            var results = userContext.Search<Series>(
                expr=>expr.Filter(s=>s.G("Books").Contains(readSeries.Books.First().Id.ToString())));
        }

        private static void AudioAndImage()
        {
            var imageData = new HttpClient()
                .GetByteArrayAsync("https://images-na.ssl-images-amazon.com/images/I/51vaI4XGL9L.jpg")
                .Result;
            var image = new Image(Convert.ToBase64String(imageData), "dummy", "image/jpeg");
            var imageFile = new ImageFile(@"C:\temp\51gKtH9dzOL.jpg");


            var audioFile = new AudioFile(@"C:\temp\BookTest\test\sommar_i_p1_20170708_0700_866b24.mp3");
            var audioData = new HttpClient()
                .GetByteArrayAsync("http://www.podtrac.com/pts/redirect.mp3/podcast.thisamericanlife.org/podcast/625.mp3")
                .Result;
            var blockSize = 200000;
            var audioDataParts = new List<Audio.AudioData>();
            while (audioData.Length > 0)
            {
                var dataPart = audioData.Take(blockSize).ToArray();
                audioData = audioData.Skip(blockSize).ToArray();
                var part = new Audio.AudioData(Convert.ToBase64String(dataPart), dataPart.Length);
                audioDataParts.Add(part);
            }
            var audio = new Audio(audioDataParts.ToArray(), "dummy", "audio/mpeg", blockSize);

            UserContext.AddDefault(audioFile);

            UserContext.AddDefault(image);
            UserContext.AddDefault(imageFile);
            UserContext.AddDefault(audio);
            foreach (var audioDataPart in audioDataParts)
            {
                UserContext.AddDefault(audioDataPart);
            }
        }

        private static void Reset()
        {
            var context = new UserContext();
            context.Reset();
        }

        private static void Delete()
        {
            var author = new Author("Axel", "Axelsson");
            var book = new Book("En bok", author);
            var series = new Series("Test", new[] { book });

            var query =
                @"query{series(id:""#####""){authors{name{fistName, lastName}} name books{id title bookAuthors{author{name{fistName lastName}}}} }}";
            query = query.Replace("#####", series.Id.ToString());

            var context = new UserContext(query);

            UserContext.AddDefault(author);
            UserContext.AddDefault(book);
            UserContext.AddDefault(series);

            //var seriesBefore = context.Search<Series>("Name", "Test", UserContext.ReadType.Shallow);
            //context.Remove<Book>(book.Id);
            //var seriesAfter = context.Search<Series>("Name", "Test", UserContext.ReadType.Shallow);
        }

        private static void NullProperty()
        {
            var book = new Book("dsisdf", null);
            var userContext = new UserContext("query{dummy{bookAuthors{author{id}}}}");
            UserContext.AddDefault(book);
            var test = userContext.Get<Book>(book.Id);
        }

        private static void Boolean()
        {
            var value = new Random().Next() > 0.5;
            var test = new BoolTest(value);
            UserContext.AddDefault(test);
            var result = UserContext.GetAllShallow<BoolTest>().First().Value;
            if(result != value)
                throw new Exception("Values should be equal");
        }

        private static void DateTime()
        {
            var date = System.DateTime.Now;
            var test = new DateTimeTest(date);
            UserContext.AddDefault(test);
            var result = UserContext.GetAllShallow<DateTimeTest>().First().Value;
            var diff = Math.Abs(result.Ticks - date.Ticks);
            if (diff > 10000)
                throw new Exception("DateTime is not working");
        }

        private static void Float()
        {
            var test = new FloatTest(0.5);
            UserContext.AddDefault(test);
            var result = UserContext.GetAllShallow<FloatTest>().First().Value;
            if(!result.Equals(0.5))
                throw new Exception("Float is not working");
        }
    }
}
