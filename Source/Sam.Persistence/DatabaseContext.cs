using Sam.FileTableFramework.Context;
using Sam.FileTableFramework.Extentions;

namespace Sam.Persistence
{
    public class DatabaseContext : FileTableDBContext
    {
        public DatabaseContext(DatabaseOptions options) : base(options)
        {
        }

        public FtDbSet Table1 { get; set; }
        public FtDbSet Table2 { get; set; }
        public FtDbSet Table3 { get; set; }
    }
}
