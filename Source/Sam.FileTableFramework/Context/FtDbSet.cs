using Sam.FileTableFramework.Context.Internall;
using Sam.FileTableFramework.Entities;
using Sam.FileTableFramework.Extentions;
using Sam.FileTableFramework.Linq;
using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.IO;
using System.Linq;
using System.Linq.Expressions;
using System.Threading.Tasks;

namespace Sam.FileTableFramework.Context
{
    public class FtDbSet
    {
        public string? TableName { get; private set; }
        public string? ConnectionString { get; private set; }
        internal ContextQuery? Query { get; set; }
        public string ToQueryString()
        {
            Query ??= new ContextQuery();

            var selectClause = string.Join(", ", Query.Fields);
            var fromClause = $"FROM {TableName}";
            var whereClause = Query.Where != null ? $"WHERE {string.Join(" AND ", Query.Where)}" : string.Empty;
            var orderByClause = Query.OrderBy != null ? $"ORDER BY {string.Join(", ", Query.OrderBy)}{(Query.OrderByDescending == true ? " desc " : "")}" : "ORDER BY name";

            var skipClause = Query.Skip.HasValue ? $"OFFSET {Query.Skip.Value} ROWS" : string.Empty;
            var takeClause = Query.Take.HasValue ? $"FETCH NEXT {Query.Take.Value} ROWS ONLY" : string.Empty;

            var queryString = $"SELECT {selectClause} {fromClause} {whereClause} {orderByClause} {skipClause} {takeClause};";

            return queryString.Trim();
        }
        public async Task<IEnumerable<FileEntity>> ToListAsync()
            => await ToListAsync(p => new FileEntity()
            {
                stream_id = p.stream_id,
                cached_file_size = p.cached_file_size,
                creation_time = p.creation_time,
                file_stream = p.file_stream,
                is_archive = p.is_archive,
                file_type = p.file_type,
                is_directory = p.is_directory,
                is_hidden = p.is_hidden,
                is_offline = p.is_offline,
                is_readonly = p.is_readonly,
                is_system = p.is_system,
                is_temporary = p.is_temporary,
                last_access_time = p.last_access_time,
                last_write_time = p.last_write_time,
                name = p.name

            });
        public async Task<IEnumerable<T>> ToListAsync<T>(Expression<Func<FileEntity, T>> selector) where T : class
        {
            try
            {
                this.Select(selector);
                using (var connection = new SqlConnection(ConnectionString))
                {
                    await connection.OpenAsync();

                    string sqlQuery = ToQueryString();
                    return await connection.GetList<T>(sqlQuery);
                }
            }
            catch (Exception)
            {
                throw;
            }
            finally
            {
                Query = null;
            }
        }
        public virtual async Task<string> CreateAsync(string fileName, Stream stream)
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
        public virtual async Task<int> RemoveAsync(FileEntity entity)
        {
            using (var connection = new SqlConnection(ConnectionString))
            {
                string sqlQuery = $"DELETE [{TableName}] WHERE [stream_id] = '{entity.stream_id}'";
                await connection.OpenAsync();

                using (var command = new SqlCommand(sqlQuery, connection))
                {
                    return await command.ExecuteNonQueryAsync();
                }
            }
        }
        public virtual async Task<int> CountAsync()
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
