using System;

namespace Sam.FileTableFramework.Data.Dto
{
    public class FileEntityDto
    {
        public Guid stream_id { get; set; }
        public string name { get; set; }
        public string file_type { get; set; }
        public long cached_file_size { get; set; }
        public DateTimeOffset creation_time { get; set; }
        public DateTimeOffset last_write_time { get; set; }
        public DateTimeOffset last_access_time { get; set; }
    }
}
