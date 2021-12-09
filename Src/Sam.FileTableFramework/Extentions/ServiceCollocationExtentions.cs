﻿using Microsoft.Extensions.DependencyInjection;
using Sam.FileTableFramework.Context;
using Sam.FileTableFramework.Data;
using System;
using System.Linq;

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

            foreach (var item in instance.GetType().GetProperties().Where(p => p.PropertyType.FullName.Equals(typeof(IRepository).FullName)))
            {
                instance.GetType().GetProperty(item.Name).SetValue(instance, new Repository(item.Name, options.ConnectionString));
            }

            services.AddSingleton(instance);
            return services;

        }
    }
    public class DatabaseOptions
    {
        public string ConnectionString { get; set; }

    }
}
