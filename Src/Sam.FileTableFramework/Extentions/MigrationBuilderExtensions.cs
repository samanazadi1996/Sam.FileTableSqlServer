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
            var masterConnectionString =new SqlConnectionStringBuilder(connectionString);
            masterConnectionString.InitialCatalog = "master";

            using (var connection = new SqlConnection(masterConnectionString.ConnectionString))
            {
                connection.Open();
                foreach (var item in context.GetType().GetProperties().Where(p => p.PropertyType.FullName.Equals(typeof(Repository).FullName)))
                {
                    try
                    {
                        //create database
                    }
                    catch { }
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
                    catch { }
                }
            }
        }
    }
}
