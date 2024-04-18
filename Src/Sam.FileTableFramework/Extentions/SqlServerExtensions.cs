using Sam.FileTableFramework.Context;
using System.Linq;

namespace Sam.FileTableFramework.Extentions
{
    public static class SqlServerExtensions
    {
        public static void UseSqlServer<T>(this T context, string connectionString) where T : FileTableDBContext
        {
            context.ConnectionString = connectionString;

            foreach (var item in context.GetType().GetProperties().Where(p => p.PropertyType.FullName.Equals(typeof(FtDbSet).FullName)))
                context.GetType().GetProperty(item.Name).SetValue(context, new FtDbSet(item.Name, context.ConnectionString!));
        }

    }
}
