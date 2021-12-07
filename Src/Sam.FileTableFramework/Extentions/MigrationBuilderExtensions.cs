using System;
using Sam.FileTableFramework.Context;
using Sam.FileTableFramework.Data;
using System.Data.SqlClient;
using System.Linq;
using Dapper;

namespace Sam.FileTableFramework.Extentions
{
    public static class MigrationBuilderExtensions
    {
        public static void GenerateDataBase(this FileTableDBContext context, string connectionString)
        {
            var masterConnectionString = new SqlConnectionStringBuilder(connectionString);
            var databaseName = masterConnectionString.InitialCatalog;
            masterConnectionString.InitialCatalog = "master";

            using (var connection = new SqlConnection(masterConnectionString.ConnectionString))
            {
                connection.Open();
                try
                {
                    var pathDatabase = connection.QueryFirst<string>("SELECT SERVERPROPERTY('INSTANCEDEFAULTDATAPATH')");

                    if (!pathDatabase.EndsWith("\\")) pathDatabase += "\\";

                    connection.Execute($@"CREATE DATABASE {databaseName} ON PRIMARY (NAME=F1,FILENAME='{pathDatabase}{databaseName}.MDF'),FILEGROUP G1 CONTAINS FILESTREAM(NAME=Str,FILENAME='{pathDatabase}{databaseName}') LOG ON (NAME=F2,FILENAME='{pathDatabase}{databaseName}Log.MDF') WITH FILESTREAM (NON_TRANSACTED_ACCESS=FULL,DIRECTORY_NAME=N'{databaseName}') ");
                }
                catch (Exception e)
                {
                    // ignored
                }

            }
        }

        public static void GenerateTables(this FileTableDBContext context, string connectionString)
        {
            using (var connection = new SqlConnection(connectionString))
            {
                connection.Open();
                foreach (var item in context.GetType().GetProperties().Where(p => p.PropertyType.FullName.Equals(typeof(Repository).FullName)))
                {
                    try
                    {
                        connection.Execute($"CREATE TABLE [{item.Name}] AS FILETABLE");
                    }
                    catch
                    {
                        // ignored
                    }
                }
            }
        }
    }
}
