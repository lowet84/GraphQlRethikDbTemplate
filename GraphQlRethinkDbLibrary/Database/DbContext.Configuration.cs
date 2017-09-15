using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using GraphQlRethinkDbLibrary.Attributes;
using GraphQlRethinkDbLibrary.Schema;

namespace GraphQlRethinkDbLibrary.Database
{
    public partial class DbContext
    {
        private enum SpecialTables
        {
            Chain
        }

        private static IEnumerable<string> DefaultTypes =>
            Assembly.GetEntryAssembly()
            .GetTypes()
            .Where(type => type.UsesDefaultDbRead())
            .Select(type=>type.GetCustomAttribute<TableAttribute>()?.TableName ?? type.Name);


        private static IEnumerable<string> TableNames =>
            DefaultTypes
            .Union(Enum.GetNames(typeof(SpecialTables)))
            .ToList();
    }
}
