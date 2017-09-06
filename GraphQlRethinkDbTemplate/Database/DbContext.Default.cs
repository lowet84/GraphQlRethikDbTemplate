using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using GraphQlRethinkDbTemplate.Attributes;
using GraphQlRethinkDbTemplate.Schema;
using GraphQlRethinkDbTemplate.Schema.Types.Converters;
using GraphQL.Conventions;
using GraphQLParser.AST;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using RethinkDb.Driver.Ast;
using RethinkDb.Driver.Model;

namespace GraphQlRethinkDbTemplate.Database
{
    public partial class DbContext
    {
        public T AddDefault<T>(T item)
        {
            var table = GetTable(typeof(T));

            var result = table.Insert(item).RunResult(_connection);
            if (result.Errors > 0)
            {
                throw new Exception("Something went wrong");
            }

            return item;
        }

        public T ReadByIdDefault<T>(Id id, GraphQLDocument document, UserContext.ReadType readType) where T : class
        {
            var selectionSet = GetSelectionSet(document);

            switch (readType)
            {
                case UserContext.ReadType.Normal:
                    return GetWithDocument<T>(selectionSet, id);
                case UserContext.ReadType.Deep:
                    throw new NotImplementedException();
                case UserContext.ReadType.Shallow:
                    return GetShallow<T>(id);
                default:
                    throw new ArgumentOutOfRangeException(nameof(readType), readType, null);
            }
        }

        private static string GetTableName(Type unsafeType)
        {
            var name = GetTypeIfArray(unsafeType).GetCustomAttribute<TableAttribute>().TableName;
            return name;
        }

        private static Table GetTable(Type type)
        {
            return R.Db(DatabaseName).Table(GetTableName(type));
        }

        private static GraphQLSelectionSet GetSelectionSet(GraphQLDocument document)
        {
            var operation =
                document.Definitions.First(d =>
                    d.Kind == ASTNodeKind.OperationDefinition) as GraphQLOperationDefinition;
            var selectionSet = (operation?.SelectionSet?.Selections?.SingleOrDefault() as GraphQLFieldSelection)?.SelectionSet;
            return selectionSet;
        }

        private T GetWithDocument<T>(GraphQLSelectionSet selectionSet, Id id) where T : class
        {
            var type = typeof(T);
            var hashMap = GetHashMap(selectionSet, type);
            try
            {
                var result = GetFromDb<T>(id, hashMap);
                var ret = Utils.DeserializeJObject<T>(result);
                return ret;
            }
            catch (Exception)
            {
                return null;
            }
        }

        private JObject GetFromDb<T>(Id id, MapObject hashMap)
        {
            var importTree = GetImportTree(typeof(T), hashMap, null);
            var table = GetTable(typeof(T));
            ReqlExpr get = table
                .Get(id.ToString());
            get = Merge(get, importTree);
            get = get.Pluck(hashMap);
            var result = get.Run(_connection) as JObject;
            return result;
        }

        private T GetShallow<T>(Id id) where T : class
        {
            var table = GetTable(typeof(T));
            try
            {
                JObject result = table.Get(id.ToString())
                    .Run(_connection);
                return Utils.DeserializeJObject<T>(result);
            }
            catch (Exception)
            {
                return null;
            }
        }

        private ReqlExpr Merge(ReqlExpr expr, ImportTreeItem importTree)
        {
            var ret = expr;

            foreach (var importItem in importTree.ImportItems)
            {
                ret = ret.Merge(item => R.HashMap(importItem.PropertyName,
                    importItem.Table.GetAll(R.Args(item.G(importItem.PropertyName)))
                    .Map(subItem => Merge(subItem,importItem))
                    .CoerceTo("ARRAY")));
            }

            return ret;
        }

        private static MapObject GetHashMap(
            GraphQLSelectionSet selectionSet,
            Type unsafeType)
        {
            var mapObject = new MapObject();
            var type = GetTypeIfArray(unsafeType);
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
                    var property = type.GetProperty(name);
                    var newType = property.PropertyType;
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

        private ImportTreeItem GetImportTree(Type unsafeType, MapObject hashMap, string rootProperty)
        {
            var type = GetTypeIfArray(unsafeType);
            var ret = new ImportTreeItem { Table = GetTable(type), PropertyName = rootProperty};
            var properties = hashMap.Select(d =>
            {
                var property = type.GetProperty(d.Key.ToString());
                return new {Property = property, HashMap = d.Value as MapObject};
            });
            var importProperties = properties.Where(d =>
                    d?.Property?.GetCustomAttribute<JsonConverterAttribute>()?.ConverterType == typeof(FromOtherTableConverter))
                .ToList();
            ret.ImportItems = importProperties
                .Select(d => GetImportTree(d.Property.PropertyType, d.HashMap, d.Property.Name)).ToList();
            return ret;
        }

        private class ImportTreeItem
        {
            public Table Table { get; set; }
            public string PropertyName { get; set; }
            public List<ImportTreeItem> ImportItems { get; set; }
        }

        private static Type GetTypeIfArray(Type type)
        {
            return type.IsArray ? type.GetElementType() : type;
        }
    }
}
