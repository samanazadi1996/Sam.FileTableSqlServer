using Dapper;
using Sam.FileTableFramework.Context;
using Sam.FileTableFramework.Data;
using Sam.FileTableFramework.Extentions.Utilities;
using System.Data.SqlClient;
using System.Linq;

namespace Sam.FileTableFramework.Extentions
{
    public static class MigrationBuilderExtensions
    {
        internal static void MigrateDatabase(this FileTableDBContext context)
        {
            GenerateDataBase(context.ConnectionString);
            GenerateTables(context);
        }
        private static void GenerateDataBase(string connectionString)
        {
            var masterConnectionString = new SqlConnectionStringBuilder(connectionString);
            var databaseName = masterConnectionString.InitialCatalog;
            masterConnectionString.InitialCatalog = "master";

            using (var connection = new SqlConnection(masterConnectionString.ConnectionString))
            {
                connection.Open();
                var existDatabase = connection.QueryFirst<int>(SqlQueriesExtention.MigrationQueries.CountOfDatabase(databaseName)) > 0;
                if (!existDatabase)
                {
                    var pathDatabase = connection.QueryFirst<string>(SqlQueriesExtention.MigrationQueries.DirectoryOfDatabases());
                    if (!pathDatabase.EndsWith("\\")) pathDatabase += "\\";
                    connection.Execute(SqlQueriesExtention.MigrationQueries.CreateDatabase(databaseName, pathDatabase));
                }
            }
        }

        private static void GenerateTables(FileTableDBContext context)
        {
            using (var connection = new SqlConnection(context.ConnectionString))
            {
                connection.Open();
                foreach (var item in context.GetType().GetProperties().Where(p => p.PropertyType.FullName.Equals(typeof(IRepository).FullName)))
                {
                    var existTable = connection.QueryFirst<int>(SqlQueriesExtention.MigrationQueries.CountOfTable(item.Name)) > 0;
                    if (!existTable)
                        connection.Execute(SqlQueriesExtention.MigrationQueries.CreateTable(item.Name));
                }
            }
        }
    }
}
