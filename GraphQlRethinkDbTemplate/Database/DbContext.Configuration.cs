using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using GraphQlRethinkDbTemplate.Attributes;
using GraphQlRethinkDbTemplate.Schema;

namespace GraphQlRethinkDbTemplate.Database
{
    public partial class DbContext
    {
        private const string DatabaseName = "GraphQlRethinkDbTemplate";
        private const string DeafultDatabase = "localhost";

        private enum SpecialTables
        {

        }

        private static IEnumerable<string> DefaultTypes =>
            Assembly.GetEntryAssembly()
            .GetTypes()
            .Where(type => type.UsesDeafultDbRead())
            .Select(type=>type.GetCustomAttribute<TableAttribute>()?.TableName ?? type.Name);


        private static IEnumerable<string> TableNames =>
            DefaultTypes
            .Union(Enum.GetNames(typeof(SpecialTables)))
            .ToList();
    }
}
