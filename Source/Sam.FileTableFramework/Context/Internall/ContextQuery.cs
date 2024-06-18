using System.Collections.Generic;

namespace Sam.FileTableFramework.Context.Internall
{
    internal class ContextQuery
    {
        public ContextQuery()
        {
            Fields = "stream_id,name,file_type,cached_file_size,creation_time,last_write_time,last_access_time".Split(",");
        }
        public string[] Fields { get; set; }
        public int? Skip { get; set; }
        public int? Take { get; set; }
        public List<string>? Where { get; set; }
        public List<string>? OrderBy { get; set; }
        public bool? OrderByDescending { get; set; }

    }
}
