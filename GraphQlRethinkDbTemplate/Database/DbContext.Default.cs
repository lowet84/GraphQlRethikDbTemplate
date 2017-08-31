using System;
using System.Linq;
using System.Reflection;
using GraphQlRethinkDbTemplate.Attributes;
using GraphQL.Conventions;
using GraphQLParser.AST;
using Newtonsoft.Json;
using RethinkDb.Driver.Model;

namespace GraphQlRethinkDbTemplate.Database
{
    public partial class DbContext
    {
        public T AddDefault<T>(T item)
        {
            var table = R.Db(DatabaseName).Table(typeof(T).Name);

            var result = table.Insert(item).RunResult(_connection);
            if (result.Errors > 0)
            {
                throw new Exception("Something went wrong");
            }

            return item;
        }

        public T ReadByIdDefault<T>(Id id, GraphQLDocument document) where T : class
        {
            var name = typeof(T).GetCustomAttribute<TableAttribute>().TableName;
            var table = R.Db(DatabaseName).Table(name);

            var operation =
                document.Definitions.First(d =>
                    d.Kind == ASTNodeKind.OperationDefinition) as GraphQLOperationDefinition;
            var selectionSet = (operation.SelectionSet.Selections.Single() as GraphQLFieldSelection).SelectionSet;
            var hashMap = GetHashMap(selectionSet, typeof(T));
            try
            {
                var ret = table.Get(id.ToString())
                        .Pluck(hashMap)
                        .RunResult<T>(_connection);
                return ret;
            }
            catch (Exception)
            {
                return null;
            }
        }

        private static MapObject GetHashMap(GraphQLSelectionSet selectionSet, Type type)
        {
            var mapObject = new MapObject();

            foreach (var selection in selectionSet.Selections)
            {
                if (!(selection is GraphQLFieldSelection fieldSelection)) continue;

                var name = GetName(fieldSelection.Name.Value, type);
                if (fieldSelection.SelectionSet == null)
                {
                    mapObject = mapObject.With(name, true);
                }
                else
                {
                    var newType = type.GetProperty(name).PropertyType;
                    mapObject = mapObject.With(name, GetHashMap(fieldSelection.SelectionSet, newType));
                }
            }

            return mapObject;
        }

        private static string GetName(string name, Type type)
        {
            var properties = type.GetProperties();
            var property = properties.First(d => string.Equals(d.Name, name, StringComparison.CurrentCultureIgnoreCase));
            var jsonProperty = property.GetCustomAttribute<JsonPropertyAttribute>();
            var ret = jsonProperty?.PropertyName ?? property.Name;
            return ret;
        }
    }
}
