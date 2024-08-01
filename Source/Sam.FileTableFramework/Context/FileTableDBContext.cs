using Sam.FileTableFramework.Extensions;

namespace Sam.FileTableFramework.Context
{
    public abstract class FileTableDbContext
    {
        internal DatabaseOptions Options { get; set; }
    }
}
