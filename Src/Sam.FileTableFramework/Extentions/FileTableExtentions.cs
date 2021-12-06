using Microsoft.Extensions.DependencyInjection;
using Sam.FileTableFramework.Context;
using Sam.FileTableFramework.Data;
using System.Linq;

namespace Sam.FileTableFramework.Extentions
{
    public static class FileTableExtentions
    {
        public static IServiceCollection AddFileTableDBContext<TData>(this IServiceCollection services, string connectionString) where TData : FileTableDBContext, new()
        {

            TData instance = new TData();

            instance.UseSqlServer(connectionString);

            foreach (var item in instance.GetType().GetProperties().Where(p => p.PropertyType.FullName.Equals(typeof(Repository).FullName)))
            {
                instance.GetType().GetProperty(item.Name).SetValue(instance, new Repository(item.Name, connectionString));
            }

            services.AddSingleton(instance);
            return services;

        }
    }
}
