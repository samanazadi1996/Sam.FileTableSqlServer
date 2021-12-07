using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace Sam.FileTableFramework.Data.Dto
{
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
