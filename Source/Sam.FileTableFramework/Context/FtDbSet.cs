using Sam.FileTableFramework.Entities;
using Sam.FileTableFramework.Extentions;
using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.ComTypes;
using System.Threading.Tasks;

namespace Sam.FileTableFramework.Context
{
    public class FtDbSet
    {
        private List<ChangeTrack>? Changes { get; set; }
        public string? TableName { get; private set; }
        public string? ConnectionString { get; private set; }

        public virtual async Task<FileEntity?> FindByNameAsync(string fileName)
        {
            using (var connection = new SqlConnection(ConnectionString))
            {
                await connection.OpenAsync();

                string sqlQuery = $"SELECT TOP 1 * FROM [{TableName}] WHERE [name] = '{fileName}'";

                return await connection.GetFirst<FileEntity>(sqlQuery);
            }
        }
        public virtual async Task<IEnumerable<FileEntityDto>> GetAllAsync()
        {
            using (var connection = new SqlConnection(ConnectionString))
            {
                await connection.OpenAsync();

                string sqlQuery = $"SELECT stream_id,name,file_type,cached_file_size,creation_time,last_write_time,last_access_time FROM [{TableName}]";
                return await connection.GetList<FileEntityDto>(sqlQuery);
            }
        }
        public virtual async Task<PagedListFileEntityDto> GetPagedListAsync(int page, int pageCount)
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
        public virtual async Task<int> CountAsync()
        {
            using (var connection = new SqlConnection(ConnectionString))
            {
                string countQuery = $"SELECT COUNT(*) FROM [{TableName}]";
                await connection.OpenAsync();

                return await connection.GetInt(countQuery);
            }
        }

        public virtual void Create(string fileName, Stream stream)
        {
            Changes ??= new List<ChangeTrack>();
            Changes.Add(ChangeTrack.Create(fileName, stream));
        }
        public virtual void Remove(FileEntity entity)
        {
            Changes ??= new List<ChangeTrack>();
            Changes.Add(ChangeTrack.Delete(entity.stream_id));
        }

        internal async Task SaveChangesAsync()
        {
            if (Changes != null)
            {
                using (var connection = new SqlConnection(ConnectionString))
                {
                    await connection.OpenAsync();
                    using (var transaction = connection.BeginTransaction())
                    {
                        try
                        {
                            foreach (var item in Changes.Where(p => p.State == Enums.EntityState.Delete))
                            {
                                string sqlQuery = $"DELETE FROM [{TableName}] WHERE [stream_id] = @StreamId";

                                using (var command = new SqlCommand(sqlQuery, connection, transaction))
                                {
                                    command.Parameters.AddWithValue("@StreamId", item.Id);
                                    await command.ExecuteNonQueryAsync();
                                }
                            }
                            foreach (var item in Changes.Where(p => p.State == Enums.EntityState.Create))
                            {
                                using (var command = new SqlCommand($"INSERT INTO {TableName} ([name],[file_stream]) VALUES (@Name, @fs)", connection, transaction))
                                {
                                    command.Parameters.AddWithValue("@Name", item.Name);
                                    command.Parameters.AddWithValue("@fs", item.Stream);

                                    await command.ExecuteNonQueryAsync();
                                }
                            }

                            transaction.Commit();
                        }
                        catch (Exception)
                        {
                            transaction.Rollback();
                            throw;
                        }
                    }
                }
            }
        }


    }
}
