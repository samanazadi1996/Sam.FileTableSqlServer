using Sam.FileTableFramework.Extentions;

namespace Sam.FileTableFramework.Context
{
    public abstract class FileTableDBContext
    {
        internal DatabaseOptions options { get; set; }
    }
}
