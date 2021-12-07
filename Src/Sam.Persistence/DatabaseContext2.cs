using Sam.FileTableFramework.Context;
using Sam.FileTableFramework.Data;

namespace Sam.Persistence
{
    public class DatabaseContext2: FileTableDBContext
    {
        public Repository Table3 { get; set; }
        public Repository Table4 { get; set; }
    }
}
