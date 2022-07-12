using Microsoft.Extensions.DependencyInjection;
using Sam.FileTableFramework.Context;
using System;

namespace Sam.FileTableFramework.Extentions
{
    public static class ServiceCollocationExtentions
    {
        public static IServiceCollection AddFileTableDBContext<TData>(this IServiceCollection services, Action<DatabaseOptions> configureOptions) where TData : FileTableDBContext, new()
        {

            DatabaseOptions options = new DatabaseOptions();
            configureOptions(options);

            TData instance = new TData();
            instance.UseSqlServer(options.ConnectionString);

            services.AddSingleton(instance);
            return services;

        }
    }
    public class DatabaseOptions
    {
        public string ConnectionString { get; set; }

    }
}
