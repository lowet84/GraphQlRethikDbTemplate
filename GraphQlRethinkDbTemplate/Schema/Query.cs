using System;
using System.Threading.Tasks;
using GraphQlRethinkDbLibrary;
using GraphQlRethinkDbLibrary.Database.Search;
using GraphQlRethinkDbTemplate.Model;
using GraphQL.Conventions;
using GraphQL.Conventions.Relay;

namespace GraphQlRethinkDbTemplate.Schema
{
    [ImplementViewer(OperationType.Query)]
    public class Query
    {
        [Description("Get author by id")]
        public Task<Author> Author(UserContext context, Id id)
        {
            var data = context.Get<Author>(id);
            return Task.FromResult(data);
        }

        [Description("Get book by id")]
        public Task<Book> Book(UserContext context, Id id)
        {
            var data = context.Get<Book>(id);
            return Task.FromResult(data);
        }

        [Description("Get a series of books")]
        public Task<Series> Series(UserContext context, Id id)
        {
            var data = context.Get<Series>(id);
            return Task.FromResult(data);
        }

        [Description("Search for books")]
        public Task<Book[]> SearchBook(UserContext context, NonNull<string> text)
        {
            var searchObject = new SearchObject<Book>()
                .Add(SearchOperationType.Match, "Title", text);
            var data = context.Search(searchObject);
            return Task.FromResult(data);
        }

        [Description("Search for series containing a specific book")]
        public Task<Series[]> SearchSeriesWithBook(UserContext context, Id bookId)
        {
            if(!bookId.IsIdentifierForType<Book>())
                throw new ArgumentException("Id is not correct for type: Book");
            var searchObject = new SearchObject<Series>()
                .Add(SearchOperationType.AnyEquals, "Books", bookId.ToString());
            var data = context.Search(searchObject);
            return Task.FromResult(data);
        }

        [Description("Get all images")]
        public Task<Image[]> AllImages(UserContext context)
        {
            var images = context.Search(new SearchObject<Image>());
            return Task.FromResult(images);
        }
    }
}
