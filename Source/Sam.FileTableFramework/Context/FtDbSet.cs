using Sam.FileTableFramework.Context.Internall;
using Sam.FileTableFramework.Entities;
using Sam.FileTableFramework.Extentions;
using Sam.FileTableFramework.Linq;
using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.IO;
using System.Linq;
using System.Linq.Expressions;
using System.Threading.Tasks;

namespace Sam.FileTableFramework.Context
{
    public class FtDbSet
    {
        public string? TableName { get; private set; }
        public string? ConnectionString { get; private set; }
        internal ContextQuery? Query { get; set; }
    }
}
