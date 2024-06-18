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
    public static class FileTableFrameworkLinq
    {
        public static FtDbSet Skip(this FtDbSet dbset, int skip)
        {
            dbset.Query ??= new Context.Internall.ContextQuery();

            dbset.Query.Skip = skip;
            return dbset;
        }
        public static FtDbSet Take(this FtDbSet dbset, int take)
        {
            dbset.Query ??= new Context.Internall.ContextQuery();

            dbset.Query.Take = take;
            return dbset;
        }
        public static FtDbSet OrderByDescending<T>(this FtDbSet dbset, Expression<Func<FileEntity, T>> keySelector)
        {
            return OrderBy(dbset, keySelector, true);
        }
        public static FtDbSet Where(this FtDbSet dbset, params string[] whereClause)
        {
            dbset.Query ??= new Context.Internall.ContextQuery();
            if (whereClause != null)
            {
                dbset.Query.Where ??= new List<string>();
                dbset.Query.Where.AddRange(whereClause);
            }

            return dbset;
        }
        public static FtDbSet OrderBy<T>(this FtDbSet dbset, Expression<Func<FileEntity, T>> keySelector, bool orderByDescending = false)
        {
            dbset.Query ??= new Context.Internall.ContextQuery();
            dbset.Query.OrderByDescending = orderByDescending;
            dbset.Query.OrderBy ??= new List<string>();

            // Extract the field name from the keySelector expression
            if (keySelector.Body is MemberExpression memberExpr)
            {
                dbset.Query.OrderBy.Add(memberExpr.Member.Name);
            }
            else if (keySelector.Body is UnaryExpression unaryExpr && unaryExpr.Operand is MemberExpression unaryMemberExpr)
            {
                dbset.Query.OrderBy.Add(unaryMemberExpr.Member.Name);
            }
            else
            {
                throw new InvalidOperationException("Unsupported expression type for OrderBy.");
            }

            return dbset;
        }
        public static FtDbSet Select<T>(this FtDbSet dbset, Expression<Func<FileEntity, T>> selector)
        {
            dbset.Query ??= new Context.Internall.ContextQuery();

            var selectList = new List<string>();

            void ProcessMemberExpression(MemberExpression memberExpr, string alias = null)
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

            dbset.Query.Fields = selectList.ToArray();
            return dbset;
        }



        // -------------------------
        public static string ToQueryString(this FtDbSet dbset)
        {
            dbset.Query ??= new ContextQuery();

            var selectClause = string.Join(", ", dbset.Query.Fields);
            var fromClause = $"FROM {dbset.TableName}";
            var whereClause = dbset.Query.Where != null ? $"WHERE {string.Join(" AND ", dbset.Query.Where)}" : string.Empty;
            var orderByClause = dbset.Query.OrderBy != null ? $"ORDER BY {string.Join(", ", dbset.Query.OrderBy)}{(dbset.Query.OrderByDescending == true ? " desc " : "")}" : "ORDER BY name";

            var skipClause = dbset.Query.Skip.HasValue ? $"OFFSET {dbset.Query.Skip.Value} ROWS" : string.Empty;
            var takeClause = dbset.Query.Take.HasValue ? $"FETCH NEXT {dbset.Query.Take.Value} ROWS ONLY" : string.Empty;

            var queryString = $"SELECT {selectClause} {fromClause} {whereClause} {orderByClause} {skipClause} {takeClause};";

            return queryString.Trim();
        }
        public static async Task<IEnumerable<FileEntity>> ToListAsync(this FtDbSet dbset) => await ToListAsync(dbset, p => new FileEntity()
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
        {
            try
            {
                dbset.Select(selector);
                using (var connection = new SqlConnection(dbset.ConnectionString))
                {
                    await connection.OpenAsync();

                    string sqlQuery = ToQueryString(dbset);
                    return await connection.GetList<T>(sqlQuery);
                }
            }
            catch (Exception)
            {
                throw;
            }
            finally
            {
                dbset.Query = null;
            }
        }
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
        public static async Task<int> CountAsync(this FtDbSet dbset)
        {
            using (var connection = new SqlConnection(dbset.ConnectionString))
            {
                string countQuery = $"SELECT COUNT(*) FROM [{dbset.TableName}]";
                await connection.OpenAsync();

                return await connection.GetInt(countQuery);
            }
        }
        public static async Task<FileEntity?> FirstOrDefaultAsync(this FtDbSet dbset) => await FirstOrDefaultAsync(dbset, p => new FileEntity()
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
        {
            try
            {
                dbset.Select(selector);
                using (var connection = new SqlConnection(dbset.ConnectionString))
                {
                    await connection.OpenAsync();

                    string sqlQuery = ToQueryString(dbset);
                    return await connection.GetFirst<T>(sqlQuery);
                }
            }
            catch (Exception)
            {
                throw;
            }
            finally
            {
                dbset.Query = null;
            }
        }
    }
}
