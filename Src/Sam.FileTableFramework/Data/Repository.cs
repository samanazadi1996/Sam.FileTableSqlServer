using Dapper;
using Sam.FileTableFramework.Dtos;
using Sam.FileTableFramework.Entities;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Threading.Tasks;

namespace Sam.FileTableFramework.Data
{
    public class Repository
    {
        private string TableName { get; set; }
        private string ConnectionString { get; set; }

        public Repository(string tableName, string connectionString)
        {
            TableName = tableName;
            ConnectionString = connectionString;
        }

        public async Task<FileEntity> FindByNameAsync(string fileName)
        {
            try
            {
                string sql = $"SELECT TOP 1 * FROM [{TableName}] WHERE [name] = '{fileName}'";

                using (var connection = new SqlConnection(ConnectionString))
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

                using (var connection = new SqlConnection(ConnectionString))
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
                return model.FileName;
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

                using (var connection = new SqlConnection(ConnectionString))
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
