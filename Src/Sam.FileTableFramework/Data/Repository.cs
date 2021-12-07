using Dapper;
using Sam.FileTableFramework.Dtos;
using Sam.FileTableFramework.Entities;
using Sam.FileTableFramework.Extentions;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Threading.Tasks;

namespace Sam.FileTableFramework.Data
{
    public class Repository
    {
        private string TableName { get; set; }
        private DatabaseOptions DatabaseOptions { get; set; }

        public Repository(string tableName, DatabaseOptions databaseOptions)
        {
            TableName = tableName;
            DatabaseOptions = databaseOptions;
        }

        public async Task<FileEntity> FindByNameAsync(string fileName)
        {
            try
            {
                string sql = $"SELECT TOP 1 * FROM [{TableName}] WHERE [name] = '{fileName}'";

                using (var connection = new SqlConnection(DatabaseOptions.ConnectionString))
                {
                    await connection.OpenAsync();

                    return await connection.QueryFirstAsync<FileEntity>(sql);
                }
            }
            catch
            {
                return null;
            }
        }

        public async Task<IEnumerable<FileEntity>> GetAllAsync()
        {
            try
            {
                string sql = $"SELECT * FROM [{TableName}]";

                using (var connection = new SqlConnection(DatabaseOptions.ConnectionString))
                {
                    await connection.OpenAsync();

                    return await connection.QueryAsync<FileEntity>(sql);
                }
            }
            catch
            {
                return null;
            }
        }
        public async Task<string> CreateAsync(CreateFileTableDto model)
        {
            try
            {
                using (var connection = new SqlConnection(DatabaseOptions.ConnectionString))
                {
                    connection.Open();
                    var sql = $"INSERT INTO {TableName} ([name],[file_stream]) VALUES ('{model.FileName}',@fs)";

                    var dParams = new DynamicParameters();
                    dParams.Add("@fs", model.Stream, DbType.Binary);

                    await connection.ExecuteAsync(sql, dParams);

                    return model.FileName;
                }
            }
            catch
            {
                return string.Empty;
            }
        }

        public async Task<int> RemoveByNameAsync(string fileName)
        {
            try
            {
                string sql = $"DELETE [{TableName}] WHERE [name] = '{fileName}'";

                using (var connection = new SqlConnection(DatabaseOptions.ConnectionString))
                {
                    await connection.OpenAsync();

                    return await connection.ExecuteAsync(sql);
                }
            }
            catch
            {
                return 0;
            }
        }
    }
}
