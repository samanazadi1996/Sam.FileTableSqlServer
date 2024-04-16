using System;
using System.Collections.Generic;
using System.Linq;

namespace Sam.FileTableFramework.Context
{
    public class FileEntityDto
    {
        public Guid stream_id { get; set; }
        public string? name { get; set; }
        public string? file_type { get; set; }
        public long cached_file_size { get; set; }
        public DateTimeOffset creation_time { get; set; }
        public DateTimeOffset last_write_time { get; set; }
        public DateTimeOffset last_access_time { get; set; }
    }
    public class PagedListFileEntityDto
    {
        public int Page { get; set; }
        public int TotalPages { get; set; }
        public int TotalItems { get; set; }
        public int PageCount { get; set; }
        public IEnumerable<FileEntityDto> Items { get; set; }
        public PagedListFileEntityDto(IEnumerable<FileEntityDto> items, int page, int pageCount, int totalItems)
        {
            Items = items;
            Page = page;
            PageCount = items.Count();
            TotalItems = totalItems;
            TotalPages = (int)Math.Ceiling(Convert.ToDecimal(totalItems) / pageCount);
        }
    }
}
