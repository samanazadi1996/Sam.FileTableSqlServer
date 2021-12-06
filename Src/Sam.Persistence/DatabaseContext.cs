using Sam.FileTableFramework.Context;
using Sam.FileTableFramework.Data;
using Sam.FileTableFramework.Entities;
using System;

namespace Sam.Persistence
{
    public class DatabaseContext: FileTableDBContext
    {
        public Repository Table1 { get; set; }
        public Repository Table2 { get; set; }
    }
}
