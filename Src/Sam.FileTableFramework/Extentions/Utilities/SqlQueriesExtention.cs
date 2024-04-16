using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Reflection;
using System.Xml.Linq;

namespace Sam.FileTableFramework.Extentions.Utilities
{
    internal class SqlQueriesExtention
    {
        internal class MigrationQueries
        {
            public static string CountOfDatabase(string name)
                => string.Format("SELECT COUNT(*) FROM SYS.DATABASES WHERE [name]='{0}'", name);
            public static string DirectoryOfDatabases()
                => "SELECT SERVERPROPERTY('INSTANCEDEFAULTDATAPATH')";
            public static string CountOfTable(string name)
                => string.Format("SELECT COUNT(*) FROM SYS.TABLES WHERE [name] = '{0}' AND [is_filetable] = 1", name);
            public static string CreateTable(string name)
                => string.Format("CREATE TABLE [{0}] AS FILETABLE", name);
            public static string CreateDatabase(string name, string path)
                => string.Format("CREATE DATABASE {0} ON PRIMARY (NAME=F1,FILENAME='{1}{0}.MDF'),FILEGROUP G1 CONTAINS FILESTREAM(NAME=Str,FILENAME='{1}{0}') LOG ON (NAME=F2,FILENAME='{1}{0}Log.MDF') WITH FILESTREAM (NON_TRANSACTED_ACCESS=FULL,DIRECTORY_NAME=N'{0}')", name, path);

        }
        internal class RepositoryQueries
        {
            public static string Find(string tableName, string fileName)
                => string.Format("SELECT TOP 1 * FROM [{0}] WHERE [name] = '{1}'", tableName, fileName);
            public static string Insert(string tableName, string fileName, string dParams)
                => string.Format("INSERT INTO {0} ([name],[file_stream]) VALUES ('{1}',{2})", tableName, fileName, dParams);
            public static string SelectPaging(string tableName, int skip, int take)
                => string.Format("SELECT *  FROM [{0}] ORDER BY name OFFSET {1} ROWS FETCH NEXT {2} ROWS ONLY", tableName, skip, take);
            public static string SelectAll(string tableName)
                => string.Format("SELECT * FROM [{0}]", tableName);
            public static string Count(string tableName)
                => string.Format("SELECT COUNT(*) FROM [{0}]", tableName);
            public static string Delete(string tableName, string fileName)
                => string.Format("DELETE [{0}] WHERE [name] = '{1}'", tableName, fileName);
        }
    }
}
