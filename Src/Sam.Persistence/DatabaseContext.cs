using Sam.FileTableFramework.Context;
using Sam.FileTableFramework.Data;
using Sam.FileTableFramework.Entities;
using System;

namespace Sam.Persistence
{
    public class DatabaseContext: FileTableDBContext
    {
        public IRepository Table1 { get; set; }
        public IRepository Table2 { get; set; }
        public IRepository Table3 { get; set; }
    }
}
