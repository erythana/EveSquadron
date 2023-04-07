using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using EVEye.DataAccess;
using EVEye.Models;
using EVEye.Models.ZKillboard.Data;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;

namespace EVEye.DependencyInjection
{
    public static class ContainerConfiguration
    {
        #region Container Initialization/Configuration

        public static IServiceCollection Configure(this IServiceCollection builder)
        {
            return builder
                .AddConfigurationOptions()
                .RegisterHttpClients()
                .RegisterSingletons()
                .RegisterByConvention();
        }

        public static IServiceCollection AddLogging(this IServiceCollection builder)
        {
            return builder.AddLogging(config =>
            {
                config.ClearProviders();
                config.SetMinimumLevel(LogLevel.Trace);
                config.AddDebug();
            });
        }
        
        private static IServiceCollection AddConfigurationOptions(this IServiceCollection builder)
        {
            var configuration = new ConfigurationBuilder()
                .AddJsonFile("appsettings.json", optional: true)
                .Build();
            builder.AddSingleton<IConfiguration>(configuration);

            return builder;
        }

        #endregion

        #region specific builder methods

        private static IServiceCollection RegisterByConvention(this IServiceCollection builder)
        {
            var typesWithConvention = Assembly.GetExecutingAssembly().GetTypes()
                .Where(t => GetMatchingInterface(t) is not null);

            foreach (var type in typesWithConvention)
                builder.AddTransient(GetMatchingInterface(type)!, type);

            return builder;
        }

        private static IServiceCollection RegisterSingletons(this IServiceCollection builder)
        {
            return builder
                .AddSingleton<ViewLocator>();
        }
        
        private static IServiceCollection RegisterHttpClients(this IServiceCollection builder)
        {
            builder
                .AddHttpClient<ZKillboardRestDataAccess<List<KillboardHistory>>>(
                    client =>
                    {
                        client.BaseAddress = new Uri("https://zkillboard.com/api/characterID/");
                        client.DefaultRequestHeaders.UserAgent.ParseAdd(ApplicationConstants.UserAgentHeader);
                        client.DefaultRequestHeaders.UserAgent.ParseAdd("Accept-Encoding: gzip");
                    });
            
            return builder;
        }

        #endregion

        #region helper methods

        private static Type? GetMatchingInterface(Type type) => type.GetInterfaces()
        .FirstOrDefault(t => t.Name == "I" + type.Name);

        #endregion
    }
}