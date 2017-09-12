using GraphQlRethinkDbTemplate.Schema;
using GraphQlRethinkDbTemplate.Schema.Types;
using GraphQL.Conventions;
using Newtonsoft.Json.Linq;
using RethinkDb.Driver.Ast;

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

        private ReqlExpr GetNewestId(ReqlExpr expr)
        {
            var chainTable = GetTable(typeof(Chain));
            var ret = chainTable
                .Filter(link => link.G("CurrentId").Eq(expr))
                .Nth(0)
                .Do_(link => chainTable
                    .Filter(subLink => subLink.G("LinkId").Eq(link.G("LinkId")))
                    .OrderBy(R.Desc("ChainVersion")))
                .Nth(0)
                .G("CurrentId");
            return ret;
        }
    }
}
