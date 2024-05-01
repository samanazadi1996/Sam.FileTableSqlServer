using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Threading.Tasks;

namespace Sam.FileTableFramework.Extentions
{
    public static class SqlQueryReaderExtensions
    {
        public static async Task<List<T>> GetList<T>(this SqlConnection connection, string sqlQuery) where T : class
        {
            var props = typeof(T).GetProperties();
            var result = new List<T>();

            using (var command = new SqlCommand(sqlQuery, connection))
            {
                using (var reader = await command.ExecuteReaderAsync())
                {
                    while (await reader.ReadAsync())
                    {
                        var obj = Activator.CreateInstance<T>();
                        foreach (var prop in props)
                        {
                            var value = reader[prop.Name]; // Get the value from the reader
                            if (value != DBNull.Value)     // Check for DBNull
                            {
                                prop.SetValue(obj, value); // Set the property value
                            }
                        }
                        result.Add(obj);
                    }
                }
            }
            return result;
        }
        public static async Task<T?> GetFirst<T>(this SqlConnection connection, string sqlQuery) where T : class
        {
            var props = typeof(T).GetProperties();

            using (var command = new SqlCommand(sqlQuery, connection))
            {
                using (var reader = await command.ExecuteReaderAsync())
                {
                    if (await reader.ReadAsync())
                    {
                        var obj = Activator.CreateInstance<T>();
                        foreach (var prop in props)
                        {
                            var value = reader[prop.Name]; // Get the value from the reader
                            if (value != DBNull.Value)     // Check for DBNull
                            {
                                prop.SetValue(obj, value); // Set the property value
                            }
                        }
                        return obj;
                    }
                }
            }
            return null;
        }
        public static async Task<int> GetInt(this SqlConnection connection, string sqlQuery)
        {
            using (var countCommand = new SqlCommand(sqlQuery, connection))
            {
                return (int)await countCommand.ExecuteScalarAsync();
            }
        }
    }
}
