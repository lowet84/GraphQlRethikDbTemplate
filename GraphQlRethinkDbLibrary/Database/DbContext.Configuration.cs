using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using GraphQlRethinkDbLibrary.Attributes;
using GraphQlRethinkDbLibrary.Schema;
using GraphQL.Conventions;

namespace GraphQlRethinkDbLibrary.Database
{
    public partial class DbContext
    {
        private static IEnumerable<TableInfo> DefaultTypes =>
            UsesDefaultDbReadTypes
            .Select(type => new TableInfo(type.GetCustomAttribute<TableAttribute>()?.TableName ?? type.Name, GetSecondaryIndexes(type)));

        private static IEnumerable<Type> UsesDefaultDbReadTypes => Assembly.GetEntryAssembly()
            .GetTypes()
            .Where(type => type.UsesDefaultDbRead());

        private static string[] GetSecondaryIndexes(Type type)
        {
            var properties = type.GetProperties().Where(d => d.GetCustomAttribute<SecondaryIndexAttribute>() != null);

            return properties.Select(d=>d.GetJPropertyName()).ToArray();
        }

        private static IEnumerable<TableInfo> TableInfos =>
            DefaultTypes
            .Union(new[] { new TableInfo("Chain", new[] { "CurrentId", "ChainVersion", "LinkId" }) })
            .ToList();

        private class TableInfo
        {
            public TableInfo(string tableName, string[] secondaryIndexes)
            {
                TableName = tableName;
                SecondaryIndexes = secondaryIndexes;
            }

            public string TableName { get; }
            public string[] SecondaryIndexes { get; }
        }
    }
}
