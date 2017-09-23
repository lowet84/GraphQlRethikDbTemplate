using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using GraphQlRethinkDbLibrary;
using GraphQlRethinkDbLibrary.Database;
using GraphQlRethinkDbLibrary.Database.Search;
using SampleApp.Model;

namespace SampleApp
{
    public static class Debug
    {
        public static void Run()
        {
            Reset();
            //Delete();
            Basic();
            //AudioAndImage();
            //Clean();
            //FixIssues();
        }

        private static void FixIssues()
        {
            var context = new UserContext();
            var search = context.Search<Series>(
                new SearchObject<Series>()
                .Add(SearchOperationType.MatchMultiple, "Name", "serie", "Andra")
                , UserContext.ReadType.Shallow);
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

            userContext.AddDefault(author);
            userContext.UpdateDefault(author2, author.Id);
            userContext.AddDefault(book);
            userContext.AddDefault(series);
            userContext.AddDefault(series2);
            userContext.UpdateDefault(newSeries, series.Id);

            var readSeries = userContext.Get<Series>(series.Id);

            var searchObject = new SearchObject<Series>()
                .Add(SearchOperationType.AnyEquals, nameof(Series.Books), readSeries.Books.First().Id.ToString());
            var results = userContext.Search(searchObject, UserContext.ReadType.WithDocument);
        }

        private static void AudioAndImage()
        {
            var userContext = new UserContext();

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

            userContext.AddDefault(audioFile);

            userContext.AddDefault(image);
            userContext.AddDefault(imageFile);
            userContext.AddDefault(audio);
            foreach (var audioDataPart in audioDataParts)
            {
                userContext.AddDefault(audioDataPart);
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

            context.AddDefault(author);
            context.AddDefault(book);
            context.AddDefault(series);

            var seriesBefore = context.Search<Series>("Name", "Test", UserContext.ReadType.Shallow);
            context.Remove<Book>(book.Id);
            var seriesAfter = context.Search<Series>("Name", "Test", UserContext.ReadType.Shallow);
        }
    }
}
