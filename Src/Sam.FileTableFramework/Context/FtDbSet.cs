using Sam.FileTableFramework.Entities;
using System;
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

                using (var command = new SqlCommand(sqlQuery, connection))
                {
                    using (var reader = await command.ExecuteReaderAsync())
                    {
                        if (await reader.ReadAsync())
                        {
                            return new FileEntity
                            {
                                stream_id = reader.GetGuid(reader.GetOrdinal("stream_id")),
                                name = reader.GetString(reader.GetOrdinal("name")),
                                file_stream = (byte[])reader["file_stream"],
                                file_type = reader.IsDBNull(reader.GetOrdinal("file_type")) ? null : reader.GetString(reader.GetOrdinal("file_type")),
                                cached_file_size = reader.GetInt64(reader.GetOrdinal("cached_file_size")),
                                creation_time = reader.GetDateTimeOffset(reader.GetOrdinal("creation_time")),
                                last_write_time = reader.GetDateTimeOffset(reader.GetOrdinal("last_write_time")),
                                last_access_time = reader.GetDateTimeOffset(reader.GetOrdinal("last_access_time")),
                                is_directory = reader.GetBoolean(reader.GetOrdinal("is_directory")),
                                is_offline = reader.GetBoolean(reader.GetOrdinal("is_offline")),
                                is_hidden = reader.GetBoolean(reader.GetOrdinal("is_hidden")),
                                is_readonly = reader.GetBoolean(reader.GetOrdinal("is_readonly")),
                                is_archive = reader.GetBoolean(reader.GetOrdinal("is_archive")),
                                is_system = reader.GetBoolean(reader.GetOrdinal("is_system")),
                                is_temporary = reader.GetBoolean(reader.GetOrdinal("is_temporary"))
                            };
                        }
                    }
                }
                return null;
            }
        }
        public async Task<IEnumerable<FileEntityDto>> GetAllAsync()
        {
            using (var connection = new SqlConnection(ConnectionString))
            {
                await connection.OpenAsync();

                string sqlQuery = $"SELECT stream_id,name,file_type,cached_file_size,creation_time,last_write_time,last_access_time FROM [{TableName}]";
                var resultList = new List<FileEntityDto>();

                using (var command = new SqlCommand(sqlQuery, connection))
                {
                    using (var reader = await command.ExecuteReaderAsync())
                    {
                        while (await reader.ReadAsync())
                        {
                            var fileEntity = new FileEntityDto
                            {
                                stream_id = reader.GetGuid(reader.GetOrdinal("stream_id")),
                                name = reader.IsDBNull(reader.GetOrdinal("name")) ? null : reader.GetString(reader.GetOrdinal("name")),
                                file_type = reader.IsDBNull(reader.GetOrdinal("file_type")) ? null : reader.GetString(reader.GetOrdinal("file_type")),
                                cached_file_size = reader.GetInt64(reader.GetOrdinal("cached_file_size")),
                                creation_time = reader.GetDateTimeOffset(reader.GetOrdinal("creation_time")),
                                last_write_time = reader.GetDateTimeOffset(reader.GetOrdinal("last_write_time")),
                                last_access_time = reader.GetDateTimeOffset(reader.GetOrdinal("last_access_time"))
                            };
                            resultList.Add(fileEntity);
                        }
                    }
                }

                return resultList;
            }
        }
        public async Task<PagedListFileEntityDto> GetPagedListAsync(int page, int pageCount)
        {
            using (var connection = new SqlConnection(ConnectionString))
            {
                await connection.OpenAsync();

                var skip = (page - 1) * pageCount;
                string pagedQuery = $"SELECT stream_id,name,file_type,cached_file_size,creation_time,last_write_time,last_access_time FROM [{TableName}] ORDER BY name OFFSET {skip} ROWS FETCH NEXT {pageCount} ROWS ONLY";

                var list = new List<FileEntityDto>();

                using (var command = new SqlCommand(pagedQuery, connection))
                {
                    using (var reader = await command.ExecuteReaderAsync())
                    {
                        while (await reader.ReadAsync())
                        {
                            var fileEntity = new FileEntityDto
                            {
                                stream_id = reader.GetGuid(reader.GetOrdinal("stream_id")),
                                name = reader.IsDBNull(reader.GetOrdinal("name")) ? null : reader.GetString(reader.GetOrdinal("name")),
                                file_type = reader.IsDBNull(reader.GetOrdinal("file_type")) ? null : reader.GetString(reader.GetOrdinal("file_type")),
                                cached_file_size = reader.GetInt64(reader.GetOrdinal("cached_file_size")),
                                creation_time = reader.GetDateTimeOffset(reader.GetOrdinal("creation_time")),
                                last_write_time = reader.GetDateTimeOffset(reader.GetOrdinal("last_write_time")),
                                last_access_time = reader.GetDateTimeOffset(reader.GetOrdinal("last_access_time"))
                            };
                            list.Add(fileEntity);
                        }
                    }
                }

                string countQuery = $"SELECT COUNT(*) FROM [{TableName}]";
                var totalItem = 0;
                using (var countCommand = new SqlCommand(countQuery, connection))
                {
                    totalItem = (int)await countCommand.ExecuteScalarAsync();
                }

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

                using (var command = new SqlCommand(countQuery, connection))
                {
                    object result = await command.ExecuteScalarAsync();

                    if (result == null && result == DBNull.Value)
                        return 0;

                    return Convert.ToInt32(result);
                }
            }
        }

    }
}
