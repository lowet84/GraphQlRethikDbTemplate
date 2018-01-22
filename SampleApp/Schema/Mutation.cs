using System;
using System.Linq;
using System.Net.Http;
using GraphQlRethinkDbLibrary;
using GraphQlRethinkDbLibrary.Schema;
using GraphQlRethinkDbLibrary.Schema.Output;
using GraphQL.Conventions;
using GraphQL.Conventions.Relay;
using SampleApp.Model;
using OperationType = GraphQL.Conventions.Relay.OperationType;

namespace SampleApp.Schema
{
    [ImplementViewer(OperationType.Mutation)]
    public class Mutation
    {
        public DefaultResult<Author> AddAuthor(
        UserContext context,
        NonNull<string> firstName,
        NonNull<string> lastName)
        {
            var author = new Author(firstName, lastName);
            var ret = UserContext.AddDefault(author);
            return new DefaultResult<Author>(ret);
        }

        public DefaultResult<Book> AddBook(
            UserContext context,
            NonNull<string> title,
            NonNull<string> authorId)
        {
            var id = new Id(authorId);
            var authorDummy = Utils.CreateDummyObject<Author>(id);
            var book = new Book(title, authorDummy);
            var ret = UserContext.AddDefault(book);
            return new DefaultResult<Book>(ret);
        }

        public DefaultResult<Series> AddSeries(
            UserContext context,
            NonNull<string> name)
        {
            var series = new Series(name, null);
            var ret = UserContext.AddDefault(series);
            return new DefaultResult<Series>(ret);
        }

        public DefaultResult<Series> AddBookToSeries(
            UserContext context,
            NonNull<string> seriesId,
            NonNull<string> bookId)
        {
            var oldSeries = UserContext.GetShallow<Series>(new Id(seriesId));
            var book = Utils.CreateDummyObject<Book>(new Id(bookId));
            var newSeries = new Series(oldSeries.Name, Utils.AddOrInitializeArray(oldSeries.Books, book));
            var ret = UserContext.UpdateDefault(newSeries, oldSeries.Id);
            return new DefaultResult<Series>(ret);
        }

        public DefaultResult<Image> DownloadImage(
            UserContext context,
            NonNull<string> imageUrl)
        {
            var result = new HttpClient().GetAsync(imageUrl).Result;
            var contentType = result.Content.Headers.ContentType;

            if (!contentType.MediaType.Contains("image"))
                return null;

            var existing = UserContext.SearchShallow<Image>("Source", imageUrl);

            if (existing?.Length > 0) return new DefaultResult<Image>(existing.Single());

            var data = result.Content.ReadAsByteArrayAsync().Result;
            var image = new Image(Convert.ToBase64String(data), imageUrl, "image/jpeg");
            UserContext.AddDefault(image);
            return new DefaultResult<Image>(image);
        }

        public DefaultResult<bool> ThisThowsAnError(UserContext context)
        {
            throw new Exception("Dummu exception");
        }
    }
}
