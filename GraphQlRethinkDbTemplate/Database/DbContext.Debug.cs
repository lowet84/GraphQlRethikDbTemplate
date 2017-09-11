using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using GraphQlRethinkDbTemplate.Schema.Model;
using GraphQlRethinkDbTemplate.Schema.Types;
using GraphQL.Conventions;

namespace GraphQlRethinkDbTemplate.Database
{
    public partial class DbContext
    {
        public void Reset()
        {
            R.DbDrop(DatabaseName).Run(_connection);
            _initialized = false;
            CheckAndPopulateIfNeeded();
        }

        public void Test(Id seriesId)
        {
            Instance.FindChainLink(seriesId);
            // TODO: fixa authors{name{fistName, lastName}}
            var query =
                @"query{series(id:""#####""){ books{title bookAuthors{author{name{fistName lastName}}} title} }}";
            query = query.Replace("#####", seriesId.ToString());
            var document = UserContext.GetDocument(query);
            var series = Instance.ReadByIdDefault<Series>(seriesId,UserContext.ReadType.Normal, document);
        }
    }
}
