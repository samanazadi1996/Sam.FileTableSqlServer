namespace Sam.FileTableFramework.Extentions.Utilities
{
    internal class SqlQueriesExtention
    {
        internal class MigrationQueries
        {
            public static string CountOfDatabase(string name)
                => $"SELECT COUNT(*) FROM SYS.DATABASES WHERE [name]='{name}'";
            public static string DirectoryOfDatabases()
                => "SELECT SERVERPROPERTY('INSTANCEDEFAULTDATAPATH')";
            public static string CountOfTable(string name)
                => $"SELECT COUNT(*) FROM SYS.TABLES WHERE [name] = '{name}' AND [is_filetable] = 1";
            public static string CreateTable(string name)
                => $"CREATE TABLE [{name}] AS FILETABLE";
            public static string CreateDatabase(string name, string path)
                =>$@"CREATE DATABASE {name} ON PRIMARY (NAME=f1, filename='{path}{name}.MDF'),
filegroup g1 CONTAINS filestream(NAME=str, filename='{path}{name}') log ON (NAME
=f2, filename='{path}{name}Log.MDF') WITH filestream (non_transacted_access=FULL
, directory_name=N'{name}') ";

        }
        internal class RepositoryQueries
        {
            public static string Find(string tableName, string fileName)
                => $"SELECT TOP 1 * FROM [{tableName}] WHERE [name] = '{fileName}'";
            public static string Insert(string tableName, string fileName, string dParams)
                => $"INSERT INTO {tableName} ([name],[file_stream]) VALUES ('{fileName}',{dParams})";
            public static string SelectPaging(string tableName, int skip, int take)
                => $"SELECT *  FROM [{tableName}] ORDER BY name OFFSET {skip} ROWS FETCH NEXT {take} ROWS ONLY";
            public static string SelectAll(string tableName)
                => $"SELECT * FROM [{tableName}]";
            public static string Count(string tableName)
                => $"SELECT COUNT(*) FROM [{tableName}]";
            public static string Delete(string tableName, string fileName)
                => $"DELETE [{tableName}] WHERE [name] = '{fileName}'";
        }
    }
}
