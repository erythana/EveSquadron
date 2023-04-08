using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Reflection;
using EVEye.DataAccess;
using EVEye.DataAccess.Interfaces;
using EVEye.Models;
using EVEye.Models.EVE.Data;
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
                .RegisterGenerics()
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
                .AddJsonFile("appsettings.json")
                .Build();
            builder.AddSingleton<IConfiguration>(configuration);

            return builder;
        }

        #endregion

        #region specific builder methods
        
        private static IServiceCollection RegisterGenerics(this IServiceCollection builder)
        {
            return builder
                .AddTransient(typeof(IEveRestDataAccess<>), typeof(EveRestDataAccess<>));
        }

        private static IServiceCollection RegisterByConvention(this IServiceCollection builder)
        {
            var typesWithConvention = Assembly.GetExecutingAssembly()
                .GetTypes()
                .Where(t => GetMatchingInterface(t) is not null &&
                            !t.IsGenericTypeDefinition);//We need to register the generics manually, otherwise it doesn't find the correct 

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
                .AddEveRestHttpClients()
                .AddZKillboardRestHttpClients();
                    
                
            
            return builder;
        }

        private static IServiceCollection AddEveRestHttpClients(this IServiceCollection builder)
        {
            builder.AddHttpClient<EveRestDataAccess<EveNameIDMapping>>(
                client =>
                {
                    client.BaseAddress = new Uri("https://esi.evetech.net/latest/universe/ids/");
                    client.DefaultRequestHeaders.UserAgent.ParseAdd(ApplicationConstants.UserAgentHeader);
                    client.DefaultRequestHeaders.AcceptEncoding.ParseAdd("gzip");
                    client.DefaultRequestHeaders.Accept.ParseAdd("application/json");
                });
            return builder;
        }
        
        private static IServiceCollection AddZKillboardRestHttpClients(this IServiceCollection builder)
        {
            builder.AddHttpClient<ZKillboardRestDataAccess<List<KillboardHistory>>>(
                client =>
                {
                    client.BaseAddress = new Uri("https://zkillboard.com/api/characterID/");
                    client.DefaultRequestHeaders.UserAgent.ParseAdd(ApplicationConstants.UserAgentHeader);
                    client.DefaultRequestHeaders.AcceptEncoding.ParseAdd("gzip");
                    client.DefaultRequestHeaders.Accept.ParseAdd("application/json");
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