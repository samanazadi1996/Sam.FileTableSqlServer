using Sam.FileTableFramework.Context;
using System;
using System.Linq;

namespace Sam.FileTableFramework.Extentions
{
    public static class SqlServerExtensions
    {
        public static void UseSqlServer<T>(this T context, string connectionString) where T : FileTableDBContext
        {
            context.ConnectionString = connectionString;

            var props = context.GetType().GetProperties().Where(p => typeof(FtDbSet).IsAssignableFrom(p.PropertyType));

            foreach (var item in props)
            {
                var ftDbSetInstance = Activator.CreateInstance(item.PropertyType);

                typeof(FtDbSet).GetProperty("TableName").SetValue(ftDbSetInstance, item.Name);
                typeof(FtDbSet).GetProperty("ConnectionString").SetValue(ftDbSetInstance, context.ConnectionString);

                context.GetType().GetProperty(item.Name).SetValue(context, ftDbSetInstance);
            }

        }
    }
}
