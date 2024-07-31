using Sam.FileTableFramework.Extensions;

namespace Sam.FileTableFramework.Context
{
    public abstract class FileTableDBContext
    {
        internal DatabaseOptions options { get; set; }
    }
}
