using System.Collections.Generic;

namespace Sam.FileTableFramework.Context.Internall
{
    internal class ContextQuery
    {
        public ContextQuery()
        {
            Fields = new string[] { "*" };
        }
        public string[] Fields { get; set; }
        public int? Skip { get; set; }
        public int? Take { get; set; }
        public List<string>? Where { get; set; }
        public List<string>? OrderBy { get; set; }
        public bool? OrderByDescending { get; set; }

    }
}
