using System;
using System.Collections.Generic;
using RethinkDb.Driver;
using RethinkDb.Driver.Net;

namespace GraphQlRethikDbTemplate.Database
{
    public partial class DbContext : IDbContext
    {
        private static DbContext _instance;
        public static DbContext Instance => _instance ?? (_instance = new DbContext());
        private static readonly RethinkDB R = RethinkDB.R;
        private readonly Connection _connection;
        private static bool _initialized;

        private DbContext()
        {
            var hostName = Environment.GetEnvironmentVariable("DATABASE") ?? DeafultDatabase;
            Console.WriteLine($"Connecting to database: {hostName}");
            _connection = R.Connection()
                .Hostname(hostName)
                .Port(RethinkDBConstants.DefaultPort)
                .Timeout(60)
                .Connect();
            CheckAndPopulateIfNeeded();
        }

        private void CheckAndPopulateIfNeeded()
        {
            if (_initialized) return;
            var list = R.DbList().Run<List<string>>(_connection);
            if (!list.Contains(DatabaseName))
            {
                R.DbCreate(DatabaseName).Run(_connection);
            }
            var tables = R.Db(DatabaseName).TableList().RunResult<List<string>>(_connection);

            foreach (var tableName in TableNames)
            {
                if (!tables.Contains(tableName))
                    R.Db(DatabaseName).TableCreate(tableName).Run(_connection);
            }
            _initialized = true;
        }
    }
}
