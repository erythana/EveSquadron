using System;
using System.Linq;
using System.Net.Http;
using System.Reflection;
using EVEye.DataAccess;
using EVEye.DataAccess.Interfaces;
using EVEye.Models;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;

namespace EVEye.DependencyInjection;

public static class ContainerConfiguration
{
    #region Container Initialization/Configuration

    public static IServiceCollection Configure(this IServiceCollection builder) => builder
        .AddConfigurationOptions()
        .RegisterSingletons()
        .RegisterByConvention()
        .RegisterHttpClients();

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

    private static IServiceCollection RegisterByConvention(this IServiceCollection builder)
    {
        var typesWithConvention = Assembly.GetExecutingAssembly()
            .GetTypes()
            .Where(t => GetMatchingInterface(t) is not null &&
                        !t.IsGenericTypeDefinition); //We need to register the generics manually, otherwise it doesn't find the correct 

        foreach (var type in typesWithConvention)
        {
            builder.AddTransient(GetMatchingInterface(type)!, type);
        }

        return builder;
    }

    private static IServiceCollection RegisterSingletons(this IServiceCollection builder) => builder
        .AddSingleton<ViewLocator>();

    private static IServiceCollection RegisterHttpClients(this IServiceCollection builder)
    {
        return builder
            .AddEveRestHttpClients()
            .AddZKillboardRestHttpClients();
    }

    private static IServiceCollection AddEveRestHttpClients(this IServiceCollection builder)
    {
        builder.AddHttpClient<IEveRestDataAccess, EveRestDataAccess>(AddDefaultRequestHeaders).ConfigurePrimaryHttpMessageHandler(() => new HttpClientHandler
        {
            MaxConnectionsPerServer = 100
        });;
        return builder;
    }

    private static IServiceCollection AddZKillboardRestHttpClients(this IServiceCollection builder)
    {
        builder.AddHttpClient<IZKillboardRestDataAccess, ZKillboardRestDataAccess>(AddDefaultRequestHeaders);
        return builder;
    }

    #endregion

    #region helper methods

    private static Type? GetMatchingInterface(Type type) => type.GetInterfaces()
        .FirstOrDefault(t => t.Name == "I" + type.Name);

    private static void AddDefaultRequestHeaders(HttpClient httpClient)
    {
        httpClient.DefaultRequestHeaders.TryAddWithoutValidation("User-Agent", ApplicationConstants.UserAgentHeader);
        httpClient.DefaultRequestHeaders.TryAddWithoutValidation("Content-Encoding", "gzip");
        httpClient.DefaultRequestHeaders.TryAddWithoutValidation("Content-Type", "application/json");
    }

    #endregion
}