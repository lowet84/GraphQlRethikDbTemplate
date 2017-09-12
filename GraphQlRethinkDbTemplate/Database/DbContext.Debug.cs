using System;
using System.Linq;
using GraphQlRethinkDbTemplate.Database.Search;
using GraphQlRethinkDbTemplate.Schema.Model;
using GraphQL.Conventions;
using GraphQLParser.AST;

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
                @"query{series(id:""#####""){authors{name{fistName, lastName}} name books{id title bookAuthors{author{name{fistName lastName}}}} }}";
            query = query.Replace("#####", seriesId.ToString());
            var document = UserContext.GetDocument(query);

            var series = Instance.ReadByIdDefault<Series>(seriesId, UserContext.ReadType.Normal, document);

            var searchObject = new SearchObject<Series>();
            searchObject.Add(SearchOperationType.AnyEquals, nameof(Series.Books), series.Books.First().Id.ToString());
            var results = Instance.Search(searchObject, document);
        }
    }
}
