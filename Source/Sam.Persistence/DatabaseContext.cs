using Sam.FileTableFramework.Context;

namespace Sam.Persistence;

public class DatabaseContext : FileTableDbContext
{
    public FtDbSet Table1 { get; set; }
    public FtDbSet Table2 { get; set; }
    public FtDbSet Table3 { get; set; }
}