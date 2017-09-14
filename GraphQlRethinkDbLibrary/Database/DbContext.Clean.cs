using System.Linq;
using System.Reflection;
using GraphQlRethinkDbLibrary.Schema;
using GraphQlRethinkDbLibrary.Schema.Types;
using GraphQL.Conventions;
using Newtonsoft.Json.Linq;
using RethinkDb.Driver.Net;

namespace GraphQlRethinkDbLibrary.Database
{
    public partial class DbContext
    {
        public void Clean()
        {
            var types = Assembly.GetEntryAssembly()
                .GetTypes()
                .Where(type => type.UsesDeafultDbRead());

            foreach (var type in types)
            {
                var table = GetTable(type);
                var cursor = table.Pluck("id").Run(_connection) as Cursor<dynamic>;
                while (cursor.MoveNext())
                {
                    string id = cursor.Current.id.ToString();
                    string latest = GetNewestId(id).Run(_connection).ToString();
                    if (id != latest)
                    {
                        table.Get(id).Delete().Run(_connection);
                    }
                }
            }
        }
    }
}
