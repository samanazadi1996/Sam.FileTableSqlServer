using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Threading.Tasks;

namespace Sam.FileTableFramework.Extensions
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
                            if (reader.HasColumn(prop.Name))
                            {
                                var ordinal = reader.GetOrdinal(prop.Name); // Get ordinal outside of IsDBNull check
                                if (!reader.IsDBNull(ordinal)) // Check for DBNull using ordinal
                                {
                                    var value = reader.GetValue(ordinal);
                                    prop.SetValue(obj, value);
                                }
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
                            if (reader.HasColumn(prop.Name))
                            {
                                var ordinal = reader.GetOrdinal(prop.Name); // Get ordinal outside of IsDBNull check
                                if (!reader.IsDBNull(ordinal)) // Check for DBNull using ordinal
                                {
                                    var value = reader.GetValue(ordinal);
                                    prop.SetValue(obj, value);
                                }
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


        private static bool HasColumn(this SqlDataReader reader, string columnName)
        {
            for (int i = 0; i < reader.FieldCount; i++)
            {
                if (reader.GetName(i).Equals(columnName, StringComparison.OrdinalIgnoreCase))
                    return true;
            }
            return false;
        }

    }
}
