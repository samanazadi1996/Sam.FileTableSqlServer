using Sam.FileTableFramework.Context;
using System.Data.SqlClient;
using System.Linq;

namespace Sam.FileTableFramework.Extentions
{
    public static class MigrationBuilderExtensions
    {
        public static void Migrate(this FileTableDBContext context)
        {
            GenerateDataBase(context.ConnectionString!);
            GenerateTables(context);
        }
        public static void GenerateTables(this FileTableDBContext context)
        {
            using (var connection = new SqlConnection(context.ConnectionString))
            {
                connection.Open();
                foreach (var item in context.GetType().GetProperties().Where(p => typeof(FtDbSet).IsAssignableFrom(p.PropertyType)))
                {
                    var existTable = TableExists(connection, item.Name);
                    if (!existTable)
                        CreateTable(connection, item.Name);
                }
            }
        }

        #region private methods
        private static void CreateTable(SqlConnection connection, string tableName)
        {
            var sqlQuery = $"CREATE TABLE [{tableName}] AS FILETABLE";
            using (var command = new SqlCommand(sqlQuery, connection))
            {
                command.ExecuteNonQuery();
            }
        }

        private static bool TableExists(SqlConnection connection, string tableName)
        {
            var sqlQuery = $"SELECT COUNT(*) FROM SYS.TABLES WHERE [name] = '{tableName}' AND [is_filetable] = 1";
            using (var command = new SqlCommand(sqlQuery, connection))
            {
                return (int)command.ExecuteScalar() > 0;
            }
        }

        private static void GenerateDataBase(string connectionString)
        {
            var masterConnectionString = new SqlConnectionStringBuilder(connectionString);
            var databaseName = masterConnectionString.InitialCatalog;
            masterConnectionString.InitialCatalog = "master";

            using (var connection = new SqlConnection(masterConnectionString.ConnectionString))
            {
                connection.Open();
                var existDatabase = DatabaseExists(connection, databaseName);
                if (!existDatabase)
                {
                    var pathDatabase = GetDatabasePath(connection);
                    if (!pathDatabase.EndsWith("\\")) pathDatabase += "\\";
                    CreateDatabase(connection, databaseName, pathDatabase);
                }

            }
        }

        private static void CreateDatabase(SqlConnection connection, string databaseName, string pathDatabase)
        {
            var sqlQuery = $@"CREATE DATABASE {databaseName} ON PRIMARY (NAME=f1, filename='{pathDatabase}{databaseName}.MDF'),
filegroup g1 CONTAINS filestream(NAME=str, filename='{pathDatabase}{databaseName}') log ON (NAME
=f2, filename='{pathDatabase}{databaseName}Log.MDF') WITH filestream (non_transacted_access=FULL
, directory_name=N'{databaseName}')";

            using (var command = new SqlCommand(sqlQuery, connection))
            {
                command.ExecuteNonQuery();
            }
        }

        private static string GetDatabasePath(SqlConnection connection)
        {
            var sqlQuery = "SELECT SERVERPROPERTY('INSTANCEDEFAULTDATAPATH')";
            using (var command = new SqlCommand(sqlQuery, connection))
            {
                return (string)command.ExecuteScalar();
            }
        }

        private static bool DatabaseExists(SqlConnection connection, string databaseName)
        {
            var sqlQuery = $"SELECT COUNT(*) FROM SYS.DATABASES WHERE [name]='{databaseName}'";
            using (var command = new SqlCommand(sqlQuery, connection))
            {
                return (int)command.ExecuteScalar() > 0;
            }
        }
        #endregion

    }
}
