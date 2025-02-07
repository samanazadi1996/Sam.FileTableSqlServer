using Sam.FileTableFramework.Context;
using System.Data.SqlClient;
using System.Linq;
using System.Threading.Tasks;

namespace Sam.FileTableFramework.Extensions
{
    public static class MigrationBuilderExtensions
    {
        public static void Migrate(this FileTableDbContext context)
        {
            GenerateDataBase(context.Options.ConnectionString!).Wait();
            GenerateTables(context).Wait();
        }
        public static async Task MigrateAsync(this FileTableDbContext context)
        {
            await GenerateDataBase(context.Options.ConnectionString!);
            await GenerateTables(context);
        }
        public static async Task GenerateTables(this FileTableDbContext context)
        {
            await using var connection = new SqlConnection(context.Options.ConnectionString);
            connection.Open();
            foreach (var item in context.GetType().GetProperties().Where(p => typeof(FtDbSet).IsAssignableFrom(p.PropertyType)))
            {
                var existTable = await TableExists(connection, item.Name);
                if (!existTable)
                    await CreateTable(connection, item.Name);
            }
        }

        #region private methods
        private static async Task CreateTable(SqlConnection connection, string tableName)
        {
            var sqlQuery = $"CREATE TABLE [{tableName}] AS FILETABLE";
            await using var command = new SqlCommand(sqlQuery, connection);
            await command.ExecuteNonQueryAsync();
        }

        private static async Task<bool> TableExists(SqlConnection connection, string tableName)
        {
            var sqlQuery = $"SELECT COUNT(*) FROM SYS.TABLES WHERE [name] = '{tableName}' AND [is_filetable] = 1";
            await using var command = new SqlCommand(sqlQuery, connection);
            return (int)await command.ExecuteScalarAsync() > 0;
        }

        private static async Task GenerateDataBase(string connectionString)
        {
            var masterConnectionString = new SqlConnectionStringBuilder(connectionString);
            var databaseName = masterConnectionString.InitialCatalog;
            masterConnectionString.InitialCatalog = "master";

            await using var connection = new SqlConnection(masterConnectionString.ConnectionString);
            connection.Open();
            var existDatabase = await DatabaseExists(connection, databaseName);
            if (!existDatabase)
            {
                var pathDatabase = await GetDatabasePath(connection);
                if (!pathDatabase.EndsWith("\\")) pathDatabase += "\\";
                await CreateDatabase(connection, databaseName, pathDatabase);
            }
        }

        private static async Task CreateDatabase(SqlConnection connection, string databaseName, string pathDatabase)
        {
            var sqlQuery = $@"CREATE DATABASE {databaseName} ON PRIMARY (NAME=f1, filename='{pathDatabase}{databaseName}.MDF'),
filegroup g1 CONTAINS filestream(NAME=str, filename='{pathDatabase}{databaseName}') log ON (NAME
=f2, filename='{pathDatabase}{databaseName}Log.MDF') WITH filestream (non_transacted_access=FULL
, directory_name=N'{databaseName}')";

            await using var command = new SqlCommand(sqlQuery, connection);
            await command.ExecuteNonQueryAsync();
        }

        private static async Task<string> GetDatabasePath(SqlConnection connection)
        {
            var sqlQuery = "SELECT SERVERPROPERTY('INSTANCEDEFAULTDATAPATH')";
            await using var command = new SqlCommand(sqlQuery, connection);
            return (string)await command.ExecuteScalarAsync();
        }

        private static async Task<bool> DatabaseExists(SqlConnection connection, string databaseName)
        {
            var sqlQuery = $"SELECT COUNT(*) FROM SYS.DATABASES WHERE [name]='{databaseName}'";
            await using var command = new SqlCommand(sqlQuery, connection);
            return (int)await command.ExecuteScalarAsync() > 0;
        }
        #endregion

    }
}
