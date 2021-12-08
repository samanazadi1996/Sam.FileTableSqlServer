using Dapper;
using Sam.FileTableFramework.Data.Dto;
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
        private string ConnectionString { get; set; }

        internal Repository(string tableName, string connectionString)
        {
            TableName = tableName;
            ConnectionString = connectionString;
        }

        public async Task<FileEntity> FindByNameAsync(string fileName)
        {
            try
            {
                string sqlQuery = $"SELECT TOP 1 * FROM [{TableName}] WHERE [name] = '{fileName}'";

                using (var connection = new SqlConnection(ConnectionString))
                {
                    await connection.OpenAsync();

                    return await connection.QueryFirstAsync<FileEntity>(sqlQuery);
                }
            }
            catch
            {
                return null;
            }
        }
        public async Task<IEnumerable<FileEntityDto>> GetAllAsync()
        {
            try
            {
                string sqlQuery = $"SELECT * FROM [{TableName}]";

                using (var connection = new SqlConnection(ConnectionString))
                {
                    await connection.OpenAsync();

                    return await connection.QueryAsync<FileEntityDto>(sqlQuery);
                }
            }
            catch
            {
                return null;
            }
        }
        public async Task<PagedListFileEntityDto> GetPagedListAsync(int page, int pageCount)
        {
            try
            {
                var skip = (page - 1) * pageCount;

                string pagedQuery = $"SELECT *  FROM [{TableName}] ORDER BY name OFFSET {skip} ROWS FETCH NEXT {pageCount} ROWS ONLY";
                string countQuery = $"SELECT COUNT(*) FROM [{TableName}]";

                using (var connection = new SqlConnection(ConnectionString))
                {
                    await connection.OpenAsync();

                    var list = await connection.QueryAsync<FileEntityDto>(pagedQuery);
                    var totalItem = await connection.QueryFirstAsync<int>(countQuery);

                    return new PagedListFileEntityDto(list, page, pageCount, totalItem);
                }
            }
            catch
            {
                return null;
            }
        }
        public async Task<string> CreateAsync(CreateFileEntityDto model)
        {
            try
            {
                using (var connection = new SqlConnection(ConnectionString))
                {
                    connection.Open();
                    var sqlQuery = $"INSERT INTO {TableName} ([name],[file_stream]) VALUES ('{model.FileName}',@fs)";

                    var dParams = new DynamicParameters();
                    dParams.Add("@fs", model.Stream, DbType.Binary);

                    await connection.ExecuteAsync(sqlQuery, dParams);

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
                string sqlQuery = $"DELETE [{TableName}] WHERE [name] = '{fileName}'";

                using (var connection = new SqlConnection(ConnectionString))
                {
                    await connection.OpenAsync();

                    return await connection.ExecuteAsync(sqlQuery);
                }
            }
            catch
            {
                return 0;
            }
        }
    }
}
