using System;
using System.Collections.Generic;
using System.Text;

namespace Sam.FileTableFramework.Entities
{
    public sealed class FileEntity
    {
        public Guid stream_id { get; set; }
        public string name { get; set; }
        public byte[] file_stream { get; set; }
        public string file_type { get; set; }
        public long cached_file_size { get; set; }
        public DateTimeOffset creation_time { get; set; }
        public DateTimeOffset last_write_time { get; set; }
        public DateTimeOffset last_access_time { get; set; }
        public bool is_directory { get; set; }
        public bool is_offline { get; set; }
        public bool is_hidden { get; set; }
        public bool is_readonly { get; set; }
        public bool is_archive { get; set; }
        public bool is_system { get; set; }
        public bool is_temporary { get; set; }
    }
}
