using System;
using System.Threading;
using System.Threading.Tasks;
using GraphQlRethinkDbLibrary.Database;
using GraphQlRethinkDbLibrary.Database.Search;
using GraphQlRethinkDbLibrary.Schema;
using GraphQlRethinkDbLibrary.Schema.Types;
using GraphQL.Conventions;
using GraphQLParser;
using GraphQLParser.AST;
using Newtonsoft.Json.Linq;

namespace GraphQlRethinkDbLibrary
{
    public class UserContext : IUserContext, IDataLoaderContextProvider
    {
        public enum ReadType
        {
            WithDocument,
            Shallow
        }

        public GraphQLDocument Document { get; }

        public UserContext() : this(null) { }

        public UserContext(string body) : this(body, null, null) { }

        public UserContext(string body, string databaseHostName, string databaseName)
        {
            if (!DbContext.Initalized)
                DbContext.Initialize(databaseHostName, databaseName);

            if (string.IsNullOrEmpty(body)) return;

            try
            {
                var query = JObject.Parse(body).GetValue("query").ToString();
                Document = GetDocument(query);
            }
            catch (Exception)
            {
                Document = GetDocument(body);
            }
        }

        public T Get<T>(Id id, ReadType readType = ReadType.WithDocument) where T : class
        {
            if (!id.IsIdentifierForType<T>())
            {
                var type = typeof(T);
                throw new ArgumentException($"Id type does not match generic type {type.Name}.");
            }

            if (typeof(T).UsesDeafultDbRead())
            {
                var data = DbContext.Instance.ReadByIdDefault<T>(id, readType, Document);
                return data;
            }
            throw new ArgumentException($"Unable to derive type from identifier '{id}'");
        }

        public T AddDefault<T>(T newItem) where T : NodeBase
        {
            return DbContext.Instance.AddDefault(newItem);
        }

        public T UpdateDefault<T>(T newItem, Id oldId) where T : NodeBase
        {
            return DbContext.Instance.AddDefault(newItem, oldId);
        }

        public T[] Search<T>(SearchObject<T> searchObject, ReadType readType = ReadType.WithDocument) where T : NodeBase
        {
            return DbContext.Instance.Search(searchObject, Document, readType);
        }

        public T[] Search<T>(string propertyName, string value, ReadType readType = ReadType.WithDocument) where T : NodeBase
        {
            var searchObject = new SearchObject<T>().Add(SearchOperationType.Match, propertyName, value);
            return Search(searchObject, readType);
        }

        public void Remove<T>(Id id)
        {
            DbContext.Instance.Remove<T>(id);
        }

        public void Restore<T>(Id id)
        {
            DbContext.Instance.Restore<T>(id);
        }

        public static GraphQLDocument GetDocument(string query)
        {
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

        public void Reset()
        {
            // ### DANGER!!!!! ###
            // This will delete your database
            DbContext.Instance.Reset();
        }
    }
}