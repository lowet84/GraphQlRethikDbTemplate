using System;
using System.Linq;
using GraphQlRethinkDbTemplate.Database.Search;
using GraphQlRethinkDbTemplate.Schema;
using GraphQlRethinkDbTemplate.Schema.Types;
using GraphQL.Conventions;
using GraphQLParser.AST;
using Microsoft.AspNetCore.Builder;
using Newtonsoft.Json.Linq;
using RethinkDb.Driver.Ast;

namespace GraphQlRethinkDbTemplate.Database
{
    public partial class DbContext
    {
        public T[] Search<T>(SearchObject<T> searchObject, GraphQLDocument document) where T: NodeBase
        {
            var type = typeof(T);
            var selectionSet = GetSelectionSet(document);
            try
            {
                ReqlExpr expr = GetTable(type);
                foreach (var operation in searchObject.Operations)
                {
                    switch (operation.OperationType)
                    {
                        case SearchOperationType.Equals:
                            expr = expr.Filter(item => item.G(operation.PropertyName).Eq(operation.Value));
                            break;
                        case SearchOperationType.Match:
                            expr = expr.Filter(item => item.G(operation.PropertyName).Match(operation.Value));
                            break;
                        case SearchOperationType.AnyEquals:
                            expr = expr.Filter(item => item.G(operation.PropertyName).Contains(operation.Value));
                            break;
                        default:
                            throw new ArgumentOutOfRangeException();
                    }
                }

                JArray result = expr
                    .Map(item => item.G("id"))
                    .Filter(id=>id.Eq(GetNewestId(id)))
                    .CoerceTo("ARRAY")
                    .Run(_connection);

                var ids = result.Select(d => new Id(d.ToString())).ToArray();
                var ret = Instance.GetWithDocument<T[]>(selectionSet, ids);
                return ret;
            }
            catch (Exception)
            {
                return null;
            }
        }
    }
}
