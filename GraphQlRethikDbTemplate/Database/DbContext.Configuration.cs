using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using GraphQlRethikDbTemplate.Attributes;
using GraphQlRethikDbTemplate.Schema;

namespace GraphQlRethikDbTemplate.Database
{
    public partial class DbContext
    {
        private const string DatabaseName = "GraphQlRethikDbTemplate";
        private const string DeafultDatabase = "localhost";

        private enum SpecialTables
        {

        }

        private static IEnumerable<string> DefaultTypes =>
            Assembly.GetEntryAssembly()
            .GetTypes()
            .Where(type => type.UsesDeafultDbRead())
            .Select(type=>type.GetCustomAttribute<TableAttribute>().TableName);


        private static IEnumerable<string> TableNames =>
            DefaultTypes
            .Union(Enum.GetNames(typeof(SpecialTables)))
            .ToList();
    }
}
