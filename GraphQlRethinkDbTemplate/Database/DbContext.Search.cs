using System.Linq;
using GraphQlRethinkDbTemplate.Schema;
using GraphQlRethinkDbTemplate.Schema.Model;
using GraphQlRethinkDbTemplate.Schema.Types;
using GraphQL.Conventions;
using Newtonsoft.Json.Linq;

namespace GraphQlRethinkDbTemplate.Database
{
    public partial class DbContext
    {
        public Chain FindChainLink(Id nodeId)
        {
            var chainTable = GetTable(typeof(Chain));
            var result = chainTable
                .Filter(link => link.G("CurrentId").Eq(nodeId.ToString()))
                .Nth(0)
                .Do_(link => chainTable
                    .Filter(subLink => subLink.G("LinkId").Eq(link.G("LinkId")))
                    .OrderBy(R.Desc("ChainVersion")))
                .Nth(0)
                .Run(_connection);
            var ret = Utils.DeserializeObject(typeof(Chain), (JToken)result) as Chain;
            return ret;
        }
    }
}
