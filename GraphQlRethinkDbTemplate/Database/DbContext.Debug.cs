using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
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
        }
    }
}
