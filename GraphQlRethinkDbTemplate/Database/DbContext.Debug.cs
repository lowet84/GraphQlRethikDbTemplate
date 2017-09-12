using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using GraphQlRethinkDbTemplate.Database.Search;
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
            var query =
                @"query{series(id:""#####""){authors{name{fistName, lastName}} name books{title bookAuthors{author{name{fistName lastName}}}} }}";
            query = query.Replace("#####", seriesId.ToString());
            var document = UserContext.GetDocument(query);
            var series = Instance.ReadByIdDefault<Series>(seriesId,UserContext.ReadType.Normal, document);
        }
    }
}
