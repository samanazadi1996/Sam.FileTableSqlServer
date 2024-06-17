using Microsoft.Extensions.DependencyInjection;
using Sam.FileTableFramework.Context;
using System;

namespace Sam.FileTableFramework.Extentions
{
    public static class ServiceCollocationExtentions
    {
        public static IServiceCollection AddFileTableDBContext<TData>(this IServiceCollection services, Action<DatabaseOptions> configureOptions) where TData : FileTableDBContext
        {
            DatabaseOptions options = new DatabaseOptions();
            configureOptions(options);

            services.AddSingleton(options);

            services.AddScoped(typeof(TData));
            return services;
        }
    }
    public class DatabaseOptions
    {
        public string? ConnectionString { get; set; }
        public DatabaseOptions()
        {

        }
        public DatabaseOptions(string connectionStrings)
        {
            ConnectionString = connectionStrings;
        }
    }
}
