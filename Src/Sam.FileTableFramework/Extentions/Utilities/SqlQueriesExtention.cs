using System;
using System.Collections.Generic;
using System.IO;
using System.Reflection;

namespace Sam.FileTableFramework.Extentions.Utilities
{
    internal class SqlQueriesExtention
    {
        private static class Resources
        {
            private static Dictionary<string, string> queries = new Dictionary<string, string>();
            internal static string Get<T>(string queryName, params object[] args)
            {
                string query;
                var querykey = typeof(T).Name + "-" + queryName;

                if (queries.ContainsKey(querykey))
                {
                    query = queries[querykey];
                }
                else
                {
                    var pathQuery = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, nameof(Resources), typeof(T).Name, queryName + ".sql");
                    query = File.ReadAllText(pathQuery);
                    queries.Add(querykey, query);
                }

                return string.Format(query, args);
            }
        }
        internal class MigrationQueries
        {
            public static string CountOfDatabase(string name) => Resources.Get<MigrationQueries>(nameof(CountOfDatabase), name);
            public static string DirectoryOfDatabases() => Resources.Get<MigrationQueries>(nameof(DirectoryOfDatabases));
            public static string CountOfTable(string name) => Resources.Get<MigrationQueries>(nameof(CountOfTable), name);
            public static string CreateTable(string name) => Resources.Get<MigrationQueries>(nameof(CreateTable), name);
            public static string CreateDatabase(string name, string path) => Resources.Get<MigrationQueries>(nameof(CreateDatabase), name, path);

        }
        internal class RepositoryQueries
        {
            public static string Find(string tableName, string fileName)
                => Resources.Get<RepositoryQueries>(nameof(Find), tableName, fileName);
            public static string Insert(string tableName, string fileName, string dParams)
                => Resources.Get<RepositoryQueries>(nameof(Insert), tableName, fileName, dParams);
            public static string SelectPaging(string tableName, int skip, int take)
                => Resources.Get<RepositoryQueries>(nameof(SelectPaging), tableName, skip, take);
            public static string SelectAll(string tableName)
                => Resources.Get<RepositoryQueries>(nameof(SelectAll), tableName);
            public static string Count(string tableName)
                => Resources.Get<RepositoryQueries>(nameof(Count), tableName);
            public static string Delete(string tableName, string fileName)
                => Resources.Get<RepositoryQueries>(nameof(Delete), tableName, fileName);
        }
    }
}
