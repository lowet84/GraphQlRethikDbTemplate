using System;
using System.Collections.Generic;
using RethinkDb.Driver;
using RethinkDb.Driver.Net;

namespace GraphQlRethinkDbLibrary.Database
{
    public partial class DbContext
    {
        private static DbContext _instance;
        internal static DbContext Instance => _instance ?? throw new Exception("DbContext is not initialized");
        private static readonly RethinkDB R = RethinkDB.R;
        private readonly Connection _connection;

        internal static  void Initialize(string databaseUrl, string databaseName)
        {
            if(_instance!= null)
                throw new Exception("DbContext is already initialized");
            DatabaseName = databaseName;
            _instance = new DbContext(databaseUrl);
        }

        private static string DatabaseName { get; set; }
        public static bool Initalized { get; private set; }

        private DbContext(string hostName)
        {
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
            if (Initalized) return;
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
            Initalized = true;
        }
    }
}
