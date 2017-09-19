using System;
using System.Linq;
using GraphQlRethinkDbLibrary.Database.Search;
using GraphQlRethinkDbLibrary.Schema.Types;
using GraphQL.Conventions;
using GraphQLParser.AST;
using Newtonsoft.Json.Linq;
using RethinkDb.Driver.Ast;

namespace GraphQlRethinkDbLibrary.Database
{
    public partial class DbContext
    {
        public T[] Search<T>(SearchObject<T> searchObject, GraphQLDocument document, UserContext.ReadType readType)
            where T : NodeBase
        {
            var type = typeof(T);
            try
            {
                ReqlExpr expr = GetTable(type);
                foreach (var operation in searchObject.Operations)
                {
                    switch (operation.OperationType)
                    {
                        case SearchOperationType.Equals:
                            expr = expr.Filter(item => item.G(operation.PropertyName).Eq(operation.Values.Single()));
                            break;
                        case SearchOperationType.Match:
                            expr = expr.Filter(item => item.G(operation.PropertyName).Match(operation.Values.Single()));
                            break;
                        case SearchOperationType.AnyEquals:
                            expr = expr.Filter(item => item.G(operation.PropertyName).Contains(operation.Values.Single()));
                            break;
                        case SearchOperationType.MatchMultiple:
                            expr = expr.Filter(item => item.G(operation.PropertyName).Match(string.Join("|", operation.Values)));
                            break;
                        default:
                            throw new ArgumentOutOfRangeException();
                    }
                }

                JArray result = expr
                    .Map(item => item.G("id"))
                    .Filter(id => id.Eq(GetNewestId(id)))
                    .CoerceTo("ARRAY")
                    .Run(_connection);

                var ids = result.Select(d => new Id(d.ToString())).ToArray();
                switch (readType)
                {
                    case UserContext.ReadType.WithDocument:
                        return Instance.GetWithDocument<T[]>(GetSelectionSet(document), ids);
                    case UserContext.ReadType.Shallow:
                        return Instance.GetShallow<T[]>(ids);
                }

                throw new NotImplementedException();
            }
            catch (Exception)
            {
                return null;
            }
        }
    }
}
