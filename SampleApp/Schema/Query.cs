using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using GraphQlRethinkDbLibrary;
using GraphQL.Conventions;
using GraphQL.Conventions.Relay;
using SampleApp.Model;

namespace SampleApp.Schema
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
            var data = context.Search<Book>("Title", text);
            return Task.FromResult(data);
        }

        [Description("Search for series containing a specific book")]
        public Task<Series[]> SearchSeriesWithBook(UserContext context, Id bookId)
        {
            if (!bookId.IsIdentifierForType<Book>())
                throw new ArgumentException("Id is not correct for type: Book");
            var data = context.Search<Series>(
                expr => expr.Filter(s => s.G("Books").Contains(bookId.ToString())));
            return Task.FromResult(data);
        }

        [Description("Get all images")]
        public Task<Node[]> AllImageIds(UserContext context)
        {
            var images = context.GetAll<Image>();
            var imageFiles = context.GetAll<ImageFile>();
            return Task.FromResult(GetNodes(images, imageFiles));
        }

        [Description("Get all audio")]
        public Task<Node[]> AllAudio(UserContext context)
        {
            var audios = context.GetAll<Audio>();
            var audioFiles = context.GetAll<AudioFile>();
            return Task.FromResult(GetNodes(audios, audioFiles));
        }

        private Node[] GetNodes(params IEnumerable<INode>[] collections)
        {
            var ret = new List<Node>();
            foreach (var collection in collections)
            {
                ret.AddRange(collection.Select(d => new Node(d)));
            }
            return ret.ToArray();
        }

        public class Node
        {
            public Node(INode nodeBase)
            {
                Id = nodeBase.Id;
            }

            public Id Id { get; }
        }
    }
}
