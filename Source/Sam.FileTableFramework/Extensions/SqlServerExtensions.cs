using Sam.FileTableFramework.Context;
using System;
using System.Linq;

namespace Sam.FileTableFramework.Extensions
{
    public static class SqlServerExtensions
    {
        public static void UseSqlServer<T>(this T context, string connectionString) where T : FileTableDbContext
            => context.UseSqlServer(new DatabaseOptions() { ConnectionString = connectionString });
        public static void UseSqlServer<T>(this T context, DatabaseOptions options) where T : FileTableDbContext
        {
            context.Options = options;

            var props = context.GetType().GetProperties().Where(p => typeof(FtDbSet).IsAssignableFrom(p.PropertyType));

            foreach (var item in props)
            {
                var ftDbSetInstance = Activator.CreateInstance(item.PropertyType);

                typeof(FtDbSet).GetProperty("TableName")?.SetValue(ftDbSetInstance, item.Name);
                typeof(FtDbSet).GetProperty("ConnectionString")?.SetValue(ftDbSetInstance, options.ConnectionString);

                context.GetType().GetProperty(item.Name)?.SetValue(context, ftDbSetInstance);
            }

        }

    }
}
