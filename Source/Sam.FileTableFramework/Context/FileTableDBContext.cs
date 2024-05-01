using System.Linq;

namespace Sam.FileTableFramework.Context
{
    public abstract class FileTableDBContext
    {
        internal string? ConnectionString { get; set; }
    }
}
