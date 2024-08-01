using Microsoft.Extensions.DependencyInjection;
using Sam.FileTableFramework.Context;
using System;

namespace Sam.FileTableFramework.Extensions
{
    public static class ServiceCollocationExtensions
    {
        public static IServiceCollection AddFileTableDbContext<TData>(this IServiceCollection services, Action<DatabaseOptions> configureOptions) where TData : FileTableDbContext, new()

        {
            DatabaseOptions options = new DatabaseOptions();
            configureOptions(options);

            TData instance = new TData();
            instance.UseSqlServer(options!);

            services.AddSingleton(instance);
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
