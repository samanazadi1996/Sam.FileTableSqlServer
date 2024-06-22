using System.Collections.Generic;

namespace Sam.FileTableFramework.Context.Internall
{
    public class ContextQuery
    {
        internal ContextQuery(FtDbSet ftDbSet)
        {
            TableName = ftDbSet.TableName!;
            ConnectionString = ftDbSet.ConnectionString!;
        }
        internal string TableName { get; set; }
        internal string ConnectionString { get; set; }
        internal List<string>? Fields { get; set; }
        internal List<string>? Where { get; set; }
        internal List<string>? OrderBy { get; set; }
        internal int? Skip { get; set; }
        internal int? Take { get; set; }
    }
}
