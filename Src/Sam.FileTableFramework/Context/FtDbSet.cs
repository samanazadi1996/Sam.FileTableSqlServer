using Sam.FileTableFramework.Entities;
using Sam.FileTableFramework.Extentions;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.IO;
using System.Threading.Tasks;

namespace Sam.FileTableFramework.Context
{
    public class FtDbSet
    {
        public string TableName { get; private set; }
        public string ConnectionString { get; private set; }

        public async Task<FileEntity?> FindByNameAsync(string fileName)
        {
            using (var connection = new SqlConnection(ConnectionString))
            {
                await connection.OpenAsync();

                string sqlQuery = $"SELECT TOP 1 * FROM [{TableName}] WHERE [name] = '{fileName}'";

                return await connection.GetFirst<FileEntity>(sqlQuery);
            }
        }
        public async Task<IEnumerable<FileEntityDto>> GetAllAsync()
        {
            using (var connection = new SqlConnection(ConnectionString))
            {
                await connection.OpenAsync();

                string sqlQuery = $"SELECT stream_id,name,file_type,cached_file_size,creation_time,last_write_time,last_access_time FROM [{TableName}]";
                return await connection.GetList<FileEntityDto>(sqlQuery);
            }
        }
        public async Task<PagedListFileEntityDto> GetPagedListAsync(int page, int pageCount)
        {
            using (var connection = new SqlConnection(ConnectionString))
            {
                await connection.OpenAsync();

                var skip = (page - 1) * pageCount;
                string pagedQuery = $"SELECT stream_id,name,file_type,cached_file_size,creation_time,last_write_time,last_access_time FROM [{TableName}] ORDER BY name OFFSET {skip} ROWS FETCH NEXT {pageCount} ROWS ONLY";

                var list = await connection.GetList<FileEntityDto>(pagedQuery);

                string countQuery = $"SELECT COUNT(*) FROM [{TableName}]";

                var totalItem = await connection.GetInt(countQuery);

                return new PagedListFileEntityDto(list, page, pageCount, totalItem);
            }
        }
        public async Task<string> CreateAsync(string fileName, Stream stream)
        {
            using (var connection = new SqlConnection(ConnectionString))
            {
                await connection.OpenAsync();

                using (var command = connection.CreateCommand())
                {
                    command.CommandText = $"INSERT INTO {TableName} ([name],[file_stream]) VALUES ('{fileName}',@fs)";
                    command.Parameters.AddWithValue("@fs", stream);

                    await command.ExecuteNonQueryAsync();
                }

                return fileName;
            }
        }
        public async Task<int> RemoveByNameAsync(string fileName)
        {
            using (var connection = new SqlConnection(ConnectionString))
            {
                string sqlQuery = $"DELETE [{TableName}] WHERE [name] = '{fileName}'";
                await connection.OpenAsync();

                using (var command = new SqlCommand(sqlQuery, connection))
                {
                    return await command.ExecuteNonQueryAsync();
                }
            }
        }
        public async Task<int> Count()
        {
            using (var connection = new SqlConnection(ConnectionString))
            {
                string countQuery = $"SELECT COUNT(*) FROM [{TableName}]";
                await connection.OpenAsync();

                return await connection.GetInt(countQuery);
            }
        }

    }
}
