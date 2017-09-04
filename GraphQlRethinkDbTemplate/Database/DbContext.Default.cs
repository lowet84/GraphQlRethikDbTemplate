using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using GraphQlRethinkDbTemplate.Attributes;
using GraphQlRethinkDbTemplate.Schema.Types.Converters;
using GraphQL.Conventions;
using GraphQLParser.AST;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Newtonsoft.Json;
using RethinkDb.Driver.Ast;
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

        public T ReadByIdDefault<T>(Id id, GraphQLDocument document, UserContext.ReadType readType) where T : class
        {
            var name = typeof(T).GetCustomAttribute<TableAttribute>().TableName;
            var table = R.Db(DatabaseName).Table(name);

            switch (readType)
            {
                case UserContext.ReadType.Normal:
                    return GetWithDocument<T>(document, table, id);
                case UserContext.ReadType.Deep:
                    throw new NotImplementedException();
                case UserContext.ReadType.Shallow:
                    return GetShallow<T>(table, id);
                default:
                    throw new ArgumentOutOfRangeException(nameof(readType), readType, null);
            }
        }

        private T GetWithDocument<T>(GraphQLDocument document, Table table, Id id) where T : class
        {
            var operation =
                document.Definitions.First(d =>
                    d.Kind == ASTNodeKind.OperationDefinition) as GraphQLOperationDefinition;
            var selectionSet = (operation.SelectionSet.Selections.Single() as GraphQLFieldSelection).SelectionSet;
            var importJob = new ImportJob();
            var hashMap = GetHashMap(selectionSet, typeof(T), importJob);
            try
            {
                var result = GetFromDb(table, id, hashMap);
                var ret = JsonConvert.DeserializeObject<T>(JsonConvert.SerializeObject(result));
                return ret;
            }
            catch (Exception)
            {
                return null;
            }
        }

        private dynamic GetFromDb(Table table, Id id, MapObject hashMap)
        {
            var result = table.Get(id.ToString())
                .Pluck(hashMap)
                .Run(_connection);
            return result;
        }

        private dynamic GetFromDb(Table table, IEnumerable<Id> ids, MapObject hashMap)
        {
            return null;
        }

        private T GetShallow<T>(Table table, Id id) where T : class
        {
            try
            {
                var ret = table.Get(id.ToString())
                    .RunResult<T>(_connection);
                return ret;
            }
            catch (Exception)
            {
                return null;
            }
        }

        private static MapObject GetHashMap(
            GraphQLSelectionSet selectionSet, 
            Type type,
            ImportJob importJob)
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
                    var property = type.GetProperty(name);
                    var converter = property.GetCustomAttribute<JsonConverterAttribute>();
                    if (converter != null && converter.ConverterType == typeof(FromOtherTableConverter))
                    {
                        mapObject = mapObject.With(name, true);

                        var propertyType = property.PropertyType;
                        if (propertyType.IsArray) propertyType = propertyType.GetElementType();
                        var tableName = propertyType.GetCustomAttribute<TableAttribute>().TableName;

                        var newType = property.PropertyType;

                        if (newType.IsArray) newType = newType.GetElementType();
                        var importMapObject = GetHashMap(fieldSelection.SelectionSet, newType, importJob);
                        importJob.AddMapObject(tableName, importMapObject, property.Name);
                    }
                    else
                    {
                        var newType = property.PropertyType;
                        mapObject = mapObject.With(name, GetHashMap(fieldSelection.SelectionSet, newType, importJob));
                    }
                    
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

        private class ImportJob
        {
            public ImportJob()
            {
                MapObjects = new List<ImportJobItem>();
            }

            public void AddMapObject(string tableName, MapObject mapObject, string propertyName)
            {
                if (!MapObjects.Any(d => d.TableName == tableName && d.PropertyName == propertyName))
                {
                    MapObjects.Add(new ImportJobItem{MapObject = mapObject, TableName = tableName, PropertyName = propertyName});
                }
            }

            private List<ImportJobItem> MapObjects { get; }
        }

        private class ImportJobItem
        {
            public string TableName { get; set; }
            public string PropertyName { get; set; }
            public MapObject MapObject { get; set; }
        }
    }
}
