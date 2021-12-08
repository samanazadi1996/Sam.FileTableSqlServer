using Dapper;
using Sam.FileTableFramework.Context;
using Sam.FileTableFramework.Data;
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
                var existDatabase = connection.QueryFirst<int>($"SELECT COUNT(*) FROM SYS.DATABASES WHERE [name]='{databaseName}'") > 0;
                if (!existDatabase)
                {
                    var pathDatabase = connection.QueryFirst<string>("SELECT SERVERPROPERTY('INSTANCEDEFAULTDATAPATH')");
                    if (!pathDatabase.EndsWith("\\")) pathDatabase += "\\";
                    connection.Execute($@"CREATE DATABASE {databaseName} ON PRIMARY (NAME=F1,FILENAME='{pathDatabase}{databaseName}.MDF'),FILEGROUP G1 CONTAINS FILESTREAM(NAME=Str,FILENAME='{pathDatabase}{databaseName}') LOG ON (NAME=F2,FILENAME='{pathDatabase}{databaseName}Log.MDF') WITH FILESTREAM (NON_TRANSACTED_ACCESS=FULL,DIRECTORY_NAME=N'{databaseName}') ");
                }
            }
        }

        private static void GenerateTables(FileTableDBContext context)
        {
            using (var connection = new SqlConnection(context.ConnectionString))
            {
                connection.Open();
                foreach (var item in context.GetType().GetProperties().Where(p => p.PropertyType.FullName.Equals(typeof(Repository).FullName)))
                {
                    var existTable = connection.QueryFirst<int>($"SELECT COUNT(*) FROM SYS.TABLES WHERE [name] = '{item.Name}' AND [is_filetable] = 1") > 0;
                    if (!existTable)
                        connection.Execute($"CREATE TABLE [{item.Name}] AS FILETABLE");
                }
            }
        }
    }
}
