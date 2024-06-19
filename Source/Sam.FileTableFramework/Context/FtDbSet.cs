using Sam.FileTableFramework.Context.Internall;

namespace Sam.FileTableFramework.Context
{
    public class FtDbSet
    {
        public string? TableName { get; private set; }
        public string? ConnectionString { get; private set; }
        internal ContextQuery? Query { get; set; }
    }
}
