﻿using System;
using System.Threading;
using System.Threading.Tasks;
using GraphQlRethinkDbTemplate.Database;
using GraphQlRethinkDbTemplate.Schema;
using GraphQL.Conventions;
using GraphQLParser;
using GraphQLParser.AST;
using Newtonsoft.Json.Linq;

namespace GraphQlRethinkDbTemplate
{
    public class UserContext : IUserContext, IDataLoaderContextProvider
    {
        public enum ReadType
        {
            Normal,
            Deep,
            Shallow
        }

        public GraphQLDocument Document { get; }

        public UserContext(string body)
        {
            if (!string.IsNullOrEmpty(body))
            {
                var query = JObject.Parse(body).GetValue("query").ToString();
                Document = GetDocument(query);
            }
        }

        public T Get<T>(Id id, ReadType readType = ReadType.Normal) where T : class
        {
            if (!id.IsIdentifierForType<T>())
            {
                throw new ArgumentException("Id type does not match generic type.");
            }

            if (typeof(T).UsesDeafultDbRead())
            {
                var data = DbContext.Instance.ReadByIdDefault<T>(id, Document, readType);
                return data;
            }
            throw new ArgumentException($"Unable to derive type from identifier '{id}'");
        }

        public T AddDefault<T>(T newItem)
        {
            return DbContext.Instance.AddDefault(newItem);
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
    }
}