using Dapper;
using Sam.FileTableFramework.Data.Dto;
using Sam.FileTableFramework.Entities;
using Sam.FileTableFramework.Extentions.Utilities;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Threading.Tasks;

namespace Sam.FileTableFramework.Data
{
    internal class Repository : IRepository
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
            using (var connection = new SqlConnection(ConnectionString))
            {
                await connection.OpenAsync();

                string sqlQuery = SqlQueriesExtention.RepositoryQueries.Find(TableName, fileName);

                return await connection.QueryFirstAsync<FileEntity>(sqlQuery);
            }
        }
        public async Task<IEnumerable<FileEntityDto>> GetAllAsync()
        {
            using (var connection = new SqlConnection(ConnectionString))
            {
                await connection.OpenAsync();

                string sqlQuery = SqlQueriesExtention.RepositoryQueries.SelectAll(TableName);
                return await connection.QueryAsync<FileEntityDto>(sqlQuery);
            }
        }
        public async Task<PagedListFileEntityDto> GetPagedListAsync(int page, int pageCount)
        {
            using (var connection = new SqlConnection(ConnectionString))
            {
                await connection.OpenAsync();

                var skip = (page - 1) * pageCount;
                string pagedQuery = SqlQueriesExtention.RepositoryQueries.SelectPaging(TableName, skip, pageCount);
                var list = await connection.QueryAsync<FileEntityDto>(pagedQuery);

                string countQuery = SqlQueriesExtention.RepositoryQueries.Count(TableName);
                var totalItem = await connection.QueryFirstAsync<int>(countQuery);

                return new PagedListFileEntityDto(list, page, pageCount, totalItem);
            }
        }
        public async Task<string> CreateAsync(CreateFileEntityDto model)
        {
            using (var connection = new SqlConnection(ConnectionString))
            {
                connection.Open();

                var dParams = new DynamicParameters();
                dParams.Add("@fs", model.Stream, DbType.Binary);

                var sqlQuery = SqlQueriesExtention.RepositoryQueries.Insert(TableName, model.FileName, "@fs");
                await connection.ExecuteAsync(sqlQuery, dParams);

                return model.FileName;
            }
        }
        public async Task<int> RemoveByNameAsync(string fileName)
        {
            using (var connection = new SqlConnection(ConnectionString))
            {
                string sqlQuery = SqlQueriesExtention.RepositoryQueries.Delete(TableName, fileName);
                await connection.OpenAsync();

                return await connection.ExecuteAsync(sqlQuery);
            }
        }

        public async Task<int> Count()
        {
            using (var connection = new SqlConnection(ConnectionString))
            {
                string countQuery = SqlQueriesExtention.RepositoryQueries.Count(TableName);
                await connection.OpenAsync();

                return await connection.QueryFirstAsync<int>(countQuery);

            }
        }
    }
}
