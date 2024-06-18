using Sam.FileTableFramework.Context.Internall;
using Sam.FileTableFramework.Entities;
using Sam.FileTableFramework.Extentions;
using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace Sam.FileTableFramework.Context
{
    public class FtDbSet
    {
        private List<ChangeTrack>? Changes { get; set; }
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
            => await ToListAsync<FileEntity>();
        public async Task<IEnumerable<T>> ToListAsync<T>() where T : class
        {
            using (var connection = new SqlConnection(ConnectionString))
            {
                await connection.OpenAsync();

                string sqlQuery = ToQueryString();
                return await connection.GetList<T>(sqlQuery);
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
