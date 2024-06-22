using Sam.FileTableFramework.Context;
using Sam.FileTableFramework.Context.Internall;
using Sam.FileTableFramework.Entities;
using Sam.FileTableFramework.Extentions;
using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.IO;
using System.Linq.Expressions;
using System.Threading.Tasks;

namespace Sam.FileTableFramework.Linq
{
    public static class FtDbSetLinq
    {
        #region Skip
        public static ContextQuery Skip(this FtDbSet dbset, int skip)
            => dbset.AsQueryable().Skip(skip);
        public static ContextQuery Skip(this ContextQuery contextQuery, int skip)
        {
            contextQuery.Skip = skip;
            return contextQuery;
        }
        #endregion

        #region Take
        public static ContextQuery Take(this FtDbSet dbset, int take)
            => dbset.AsQueryable().Take(take);
        public static ContextQuery Take(this ContextQuery contextQuery, int take)
        {
            contextQuery.Take = take;
            return contextQuery;
        }
        #endregion

        #region OrderBy
        public static ContextQuery OrderBy<T>(this FtDbSet dbset, Expression<Func<FileEntity, T>> keySelector)
            => dbset.AsQueryable().OrderBy(keySelector);
        public static ContextQuery OrderBy<T>(this ContextQuery contextQuery, Expression<Func<FileEntity, T>> keySelector)
        {

            if (keySelector.Body is MemberExpression memberExpr)
            {
                return contextQuery.OrderBy(memberExpr.Member.Name);
            }
            else if (keySelector.Body is UnaryExpression unaryExpr && unaryExpr.Operand is MemberExpression unaryMemberExpr)
            {
                return contextQuery.OrderBy(unaryMemberExpr.Member.Name);
            }
            else
            {
                throw new InvalidOperationException("Unsupported expression type for OrderBy.");
            }
        }
        public static ContextQuery OrderBy(this FtDbSet dbset, string fieldName)
            => OrderBy(dbset.AsQueryable(), fieldName);
        public static ContextQuery OrderBy(this ContextQuery contextQuery, string fieldName)
        {
            contextQuery.OrderBy ??= new List<string>();

            contextQuery.OrderBy.Add(fieldName);

            return contextQuery;
        }
        #endregion

        #region OrderByDescending
        public static ContextQuery OrderByDescending<T>(this FtDbSet dbset, Expression<Func<FileEntity, T>> keySelector)
            => dbset.AsQueryable().OrderByDescending(keySelector);
        public static ContextQuery OrderByDescending<T>(this ContextQuery contextQuery, Expression<Func<FileEntity, T>> keySelector)
        {
            if (keySelector.Body is MemberExpression memberExpr)
            {
                return contextQuery.OrderByDescending(memberExpr.Member.Name);
            }
            else if (keySelector.Body is UnaryExpression unaryExpr && unaryExpr.Operand is MemberExpression unaryMemberExpr)
            {
                return contextQuery.OrderByDescending(unaryMemberExpr.Member.Name);
            }
            else
            {
                throw new InvalidOperationException("Unsupported expression type for OrderBy.");
            }
        }
        public static ContextQuery OrderByDescending(this FtDbSet dbset, string fieldName)
            => OrderByDescending(dbset.AsQueryable(), fieldName);
        public static ContextQuery OrderByDescending(this ContextQuery contextQuery, string fieldName)
        {
            contextQuery.OrderBy ??= new List<string>();

            contextQuery.OrderBy.Add(fieldName + " DESC");

            return contextQuery;
        }
        #endregion

        #region Where
        public static ContextQuery Where(this FtDbSet dbset, params string[] whereClause)
            => dbset.AsQueryable().Where(whereClause);
        public static ContextQuery Where(this ContextQuery contextQuery, params string[] whereClause)
        {
            if (whereClause != null)
            {
                contextQuery.Where ??= new List<string>();
                contextQuery.Where.AddRange(whereClause);
            }

            return contextQuery;
        }
        #endregion

        #region Select
        public static ContextQuery Select<T>(this FtDbSet dbset, Expression<Func<FileEntity, T>> selector)
            => dbset.AsQueryable().Select(selector);
        public static ContextQuery Select<T>(this ContextQuery contextQuery, Expression<Func<FileEntity, T>> selector)
        {
            var selectList = new List<string>();

            void ProcessMemberExpression(MemberExpression memberExpr, string? alias = null)
            {
                var fieldName = memberExpr.Member.Name;
                alias ??= memberExpr.Member.Name;
                selectList.Add($"{fieldName} AS {alias}");
            }

            try
            {
                switch (selector.Body)
                {
                    case NewExpression newExpr:
                        for (int i = 0; i < newExpr.Arguments.Count; i++)
                        {
                            if (newExpr.Arguments[i] is MemberExpression memberExpr)
                            {
                                var alias = newExpr.Members[i].Name;
                                ProcessMemberExpression(memberExpr, alias);
                            }
                            else if (newExpr.Arguments[i] is UnaryExpression unaryExpr && unaryExpr.Operand is MemberExpression unaryMemberExpr)
                            {
                                var alias = newExpr.Members[i].Name;
                                ProcessMemberExpression(unaryMemberExpr, alias);
                            }
                            else
                            {
                                throw new InvalidOperationException($"Unsupported expression type in NewExpression at index {i}: {newExpr.Arguments[i].GetType().Name}");
                            }
                        }
                        break;

                    case MemberExpression memberExpr:
                        ProcessMemberExpression(memberExpr);
                        break;

                    case UnaryExpression unaryExpr when unaryExpr.Operand is MemberExpression unaryMemberExpr:
                        ProcessMemberExpression(unaryMemberExpr);
                        break;

                    case MemberInitExpression memberInitExpr:
                        foreach (var binding in memberInitExpr.Bindings)
                        {
                            if (binding is MemberAssignment assignment && assignment.Expression is MemberExpression assignmentMemberExpr)
                            {
                                var alias = binding.Member.Name;
                                ProcessMemberExpression(assignmentMemberExpr, alias);
                            }
                            else if (binding is MemberAssignment assignmentUnary && assignmentUnary.Expression is UnaryExpression unaryAssignmentExpr && unaryAssignmentExpr.Operand is MemberExpression unaryAssignmentMemberExpr)
                            {
                                var alias = binding.Member.Name;
                                ProcessMemberExpression(unaryAssignmentMemberExpr, alias);
                            }
                            else
                            {
                                throw new InvalidOperationException($"Unsupported expression type in MemberInitExpression: {binding.GetType().Name}");
                            }
                        }
                        break;

                    default:
                        throw new InvalidOperationException($"Unsupported expression type: {selector.Body.GetType().Name}");
                }
            }
            catch (Exception ex)
            {
                throw new InvalidOperationException("Failed to parse selector expression.", ex);
            }

            contextQuery.Fields = selectList;
            return contextQuery;
        }
        #endregion

        #region General
        public static ContextQuery AsQueryable(this FtDbSet dbset)
            => new ContextQuery(dbset);
        public static string ToQueryString(this FtDbSet dbset)
            => dbset.AsQueryable().ToQueryString();
        public static string ToQueryString(this ContextQuery contextQuery)
        {
            var selectClause = string.Join(", ", contextQuery.Fields);
            var fromClause = $"FROM {contextQuery.TableName}";
            var whereClause = contextQuery.Where != null ? $"WHERE {string.Join(" AND ", contextQuery.Where)}" : string.Empty;
            var orderByClause = contextQuery.OrderBy != null ? $"ORDER BY {string.Join(", ", contextQuery.OrderBy)}" : string.Empty;

            var skipClause = contextQuery.Skip.HasValue ? $"OFFSET {contextQuery.Skip.Value} ROWS" : string.Empty;
            var takeClause = contextQuery.Take.HasValue ? $"FETCH NEXT {contextQuery.Take.Value} ROWS ONLY" : string.Empty;

            var queryString = $"SELECT {selectClause} {fromClause} {whereClause} {orderByClause} {skipClause} {takeClause}";

            return queryString.Trim();
        }
        #endregion

        #region ToListAsync
        public static async Task<IEnumerable<FileEntity>> ToListAsync(this FtDbSet dbset)
            => await dbset.AsQueryable().ToListAsync();
        public static async Task<IEnumerable<FileEntity>> ToListAsync(this ContextQuery contextQuery)
            => await contextQuery.ToListAsync(p => new FileEntity()
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
        public static async Task<IEnumerable<T>> ToListAsync<T>(this FtDbSet dbset, Expression<Func<FileEntity, T>> selector) where T : class
            => await dbset.AsQueryable().ToListAsync(selector);
        public static async Task<IEnumerable<T>> ToListAsync<T>(this ContextQuery contextQuery, Expression<Func<FileEntity, T>> selector) where T : class
        {
            contextQuery.Select(selector);
            using (var connection = new SqlConnection(contextQuery.ConnectionString))
            {
                await connection.OpenAsync();

                string sqlQuery = contextQuery.ToQueryString();
                return await connection.GetList<T>(sqlQuery);
            }
        }
        #endregion

        #region CountAsync
        public static async Task<int> CountAsync(this FtDbSet dbset)
            => await dbset.AsQueryable().CountAsync();
        public static async Task<int> CountAsync(this ContextQuery contextQuery)
        {

            using (var connection = new SqlConnection(contextQuery.ConnectionString))
            {
                var temp = contextQuery.Fields;

                contextQuery.Fields = new List<string>() { "COUNT(stream_id)" };

                string sqlQuery = contextQuery.ToQueryString();

                contextQuery.Fields = temp;

                await connection.OpenAsync();

                return await connection.GetInt(sqlQuery);
            }
        }
        #endregion

        #region FirstOrDefaultAsync
        public static async Task<FileEntity?> FirstOrDefaultAsync(this FtDbSet dbset)
            => await dbset.AsQueryable().FirstOrDefaultAsync();
        public static async Task<FileEntity?> FirstOrDefaultAsync(this ContextQuery contextQuery)
            => await contextQuery.FirstOrDefaultAsync(p => new FileEntity()
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
        public static async Task<T?> FirstOrDefaultAsync<T>(this FtDbSet dbset, Expression<Func<FileEntity, T>> selector) where T : class
            => await dbset.FirstOrDefaultAsync<T>(selector);
        public static async Task<T?> FirstOrDefaultAsync<T>(this ContextQuery contextQuery, Expression<Func<FileEntity, T>> selector) where T : class
        {
            contextQuery.Select(selector);

            using (var connection = new SqlConnection(contextQuery.ConnectionString))
            {
                await connection.OpenAsync();

                string sqlQuery = contextQuery.ToQueryString().Insert(6, " TOP (1)");

                return await connection.GetFirst<T>(sqlQuery);
            }
        }
        #endregion

        #region CreateAsync
        public static async Task<string> CreateAsync(this FtDbSet dbset, string fileName, Stream stream)
        {
            using (var connection = new SqlConnection(dbset.ConnectionString))
            {
                await connection.OpenAsync();

                using (var command = connection.CreateCommand())
                {
                    command.CommandText = $"INSERT INTO {dbset.TableName} ([name],[file_stream]) VALUES ('{fileName}',@fs)";
                    command.Parameters.AddWithValue("@fs", stream);

                    await command.ExecuteNonQueryAsync();
                }

                return fileName;
            }
        }
        #endregion

        #region RemoveAsync
        public static async Task<int> RemoveAsync(this FtDbSet dbset, FileEntity entity)
        {
            using (var connection = new SqlConnection(dbset.ConnectionString))
            {
                string sqlQuery = $"DELETE [{dbset.TableName}] WHERE [stream_id] = '{entity.stream_id}'";
                await connection.OpenAsync();

                using (var command = new SqlCommand(sqlQuery, connection))
                {
                    return await command.ExecuteNonQueryAsync();
                }
            }
        }
        #endregion

    }
}
