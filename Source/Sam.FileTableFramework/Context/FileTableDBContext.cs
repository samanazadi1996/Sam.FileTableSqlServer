using Sam.FileTableFramework.Extentions;
using System;
using System.Linq;
using System.Threading.Tasks;

namespace Sam.FileTableFramework.Context
{
    public abstract class FileTableDBContext
    {
        internal DatabaseOptions options { get; set; }
        public FileTableDBContext(DatabaseOptions options)
        {
            this.options = options;

            var props = GetType().GetProperties().Where(p => typeof(FtDbSet).IsAssignableFrom(p.PropertyType));

            foreach (var item in props)
            {
                var ftDbSetInstance = Activator.CreateInstance(item.PropertyType);

                typeof(FtDbSet).GetProperty("TableName").SetValue(ftDbSetInstance, item.Name);
                typeof(FtDbSet).GetProperty("ConnectionString").SetValue(ftDbSetInstance, options.ConnectionString);

                GetType().GetProperty(item.Name).SetValue(this, ftDbSetInstance);
            }

        }
    }
}
