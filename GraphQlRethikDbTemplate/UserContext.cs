using System;
using System.Threading;
using System.Threading.Tasks;
using GraphQlRethikDbTemplate.Database;
using GraphQlRethikDbTemplate.Schema;
using GraphQL.Conventions;
using GraphQLParser;
using GraphQLParser.AST;
using Newtonsoft.Json.Linq;

namespace GraphQlRethikDbTemplate
{
    public class UserContext : IUserContext, IDataLoaderContextProvider
    {
        public GraphQLDocument Document { get; }

        public UserContext(string body)
        {
            if (!string.IsNullOrEmpty(body))
            {
                Document = GetDocument(body);
            }
        }
        //private readonly IBookRepository _bookRepository = new BookRepository();

        //private readonly IAuthorRepository _authorRepository = new AuthorRepository();

        public T Get<T>(Id id) where T : class
        {
            if (!id.IsIdentifierForType<T>())
            {
                throw new ArgumentException("Id type does not match generic type.");
            }

            if (typeof(T).UsesDeafultDbRead())
            {
                var data = DbContext.Instance.ReadByIdDefault<T>(id, Document);
                return data;
            }
            throw new ArgumentException($"Unable to derive type from identifier '{id}'");
        }

        //public IEnumerable<INode> Search(string searchString)
        //{
        //    //foreach (var dto in _bookRepository.SearchForBooksByTitle(searchString))
        //    //{
        //    //    yield return new Book(dto);
        //    //}
        //    //foreach (var dto in _authorRepository.SearchForAuthorsByLastName(searchString))
        //    //{
        //    //    yield return new Author(dto);
        //    //}
        //    throw new NotImplementedException();
        //}

        private GraphQLDocument GetDocument(string body)
        {
            var query = JObject.Parse(body).GetValue("query").ToString();
            var lexer = new Lexer();
            var parser = new Parser(lexer);
            var source = new Source(query);
            var document = parser.Parse(source);
            return document;
        }

        public Task FetchData(CancellationToken token)
        {
            return Task.CompletedTask;
        }
    }
}