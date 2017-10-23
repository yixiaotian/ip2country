using System.Collections.Generic;
using System.Data.Common;
using System.Data.SQLite;
using System.IO;
using System.Linq;
using System.Reflection;
using Dapper;
using IP2Country.Dto;
using IP2Country.Net;

namespace IP2Country.RirStatsDataExporters
{
    public class SQLiteRirStatsDataExporter : IRirStatsDataExporter
    {
        public string Extension { get; } = ".db";

        public string Export(IReadOnlyCollection<RirStatsListDto> items)
        {
            var tmp = Path.GetTempFileName();
            using (var ms = Assembly.GetExecutingAssembly().GetManifestResourceStream("IP2Country.IP2Country.db"))
            {
                using (var fs = File.OpenWrite(tmp))
                {
                    ms?.CopyTo(fs);
                }
            }
            using (var conn = CreateDbConnection(tmp))
            {
                const string sql =
                    "INSERT INTO IP2Country " +
                    "(BeginIPAddress, EndIPAddress, BeginIPAddressCode, EndIPAddressCode, Value, Registry, Country, Date, Status)" +
                    " VALUES " +
                    "(@BeginIPAddress, @EndIPAddress, @BeginIPAddressCode, @EndIPAddressCode, @Value, @Registry, @Country, @Date, @Status)";
                conn.Open();
                using (var tran = conn.BeginTransaction())
                {
                    conn.Execute(sql, items.Select(i => new
                    {
                        BeginIPAddress = i.BeginIPAddress.ToString(),
                        EndIPAddress = i.EndIPAddress.ToString(),
                        BeginIPAddressCode = i.BeginIPAddress.GetCode(),
                        EndIPAddressCode = i.EndIPAddress.GetCode(),
                        i.Value,
                        i.Registry,
                        i.Country,
                        i.Date,
                        i.Status
                    }));
                    tran.Commit();
                }
            }
            return tmp;
        }

        private DbConnection CreateDbConnection(string path)
        {
            var builder = new SQLiteConnectionStringBuilder
            {
                DataSource = path,
                //Pooling = true,
                //CacheSize = 512 * 1024 * 1024 / -1024,
                //SyncMode = SynchronizationModes.Off,
                //JournalMode = SQLiteJournalModeEnum.Wal,
                //PrepareRetries = 10
            };
            return new SQLiteConnection(builder.ConnectionString);
        }
    }
}