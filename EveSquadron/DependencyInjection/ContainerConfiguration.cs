using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Reflection;
using EveSquadron.DataAccess;
using EveSquadron.DataAccess.Interfaces;
using EveSquadron.Models;
using EveSquadron.Models.Interfaces;
using EveSquadron.Models.Options;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;

namespace EveSquadron.DependencyInjection;

public static class ContainerConfiguration
{
    private static IConfigurationBuilder GetConfigurationBuilderDefaults()
    {
        return new ConfigurationBuilder()
            .SetBasePath(Directory.GetCurrentDirectory())
            .AddJsonFile("appsettings.json")
            .AddJsonFile("appsettings.Logging.json");
    }

    #region Container Initialization/Configuration

    public static IServiceCollection ConfigureMinimal(this IServiceCollection builder)
    {
        var minimalConfiguration = GetConfigurationBuilderDefaults().Build();
        return builder
            .AddConfiguration(minimalConfiguration)
            .AddLogging(minimalConfiguration);
    }

    public static IServiceCollection Configure(this IServiceCollection builder)
    {
        var minimalConfiguration = GetConfigurationBuilderDefaults()
            .Build();

        var configuration = new ConfigurationBuilder()
            .AddConfiguration(minimalConfiguration)
            .Add(new SqLiteConfigurationSource(minimalConfiguration))
            .Build();

        return builder.ConfigureMinimal()
            .AddOptionMappings(configuration)
            .RegisterTransientsByConvention()
            .RegisterTransientsNonConvention()
            .RegisterSingletons()
            .RegisterHttpClients();
    }


    private static IServiceCollection AddConfiguration(this IServiceCollection builder, IConfiguration configuration)
    {
        builder.AddSingleton(configuration);
        return builder;
    }

    private static IServiceCollection AddOptionMappings(this IServiceCollection builder, IConfiguration configuration)
    {
        AddOptionAndValidateDataAnnotations<EveSquadronOptions>(configuration.GetSection(EveSquadronOptions.Section));
        AddOptionAndValidateDataAnnotations<StatusOptions>(configuration.GetSection(StatusOptions.Section));
        AddOptionAndValidateDataAnnotations<ReleaseEndpointOptions>(configuration.GetSection(ReleaseEndpointOptions.Section));
        AddOptionAndValidateDataAnnotations<ZkillboardEndpointOptions>(configuration.GetSection(ZkillboardEndpointOptions.Section));
        AddOptionAndValidateDataAnnotations<EveEndpointOptions>(configuration.GetSection(EveEndpointOptions.Section));
        return builder;

        void AddOptionAndValidateDataAnnotations<T>(IConfiguration config) where T : class => builder
            .AddOptions<T>()
            .Bind(config)
            .ValidateDataAnnotations();
    }

    private static IServiceCollection AddLogging(this IServiceCollection builder, IConfiguration configuration)
    {
        var loggingConfiguration = configuration.GetSection("Logging");

        return builder.AddLogging(config =>
        {
            config.ClearProviders();
            config.SetMinimumLevel(LogLevel.Trace);
            config.AddConfiguration(loggingConfiguration);
            config.AddDebug();
            config.AddConsole();
            if (OperatingSystem.IsWindows())
                config.AddEventLog();
        });
    }

    #endregion

    #region specific builder methods

    private static IServiceCollection RegisterTransientsByConvention(this IServiceCollection builder)
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

    private static IServiceCollection RegisterTransientsNonConvention(this IServiceCollection builder) => builder
        .AddTransient<IReleaseVersionChecker, GithubReleaseVersionChecker>()
        .AddTransient(typeof(IClipboardToWhitelistEntitiesParser<>), typeof(ClipboardToWhitelistEntitiesParser<>));


    private static IServiceCollection RegisterSingletons(this IServiceCollection builder) => builder
        .AddSingleton<IEqualityComparer<IWhitelistEntry>, WhitelistEntityEqualityComparer>();

    private static IServiceCollection RegisterHttpClients(this IServiceCollection builder)
    {
        return builder
            .AddEveRestHttpClients()
            .AddZKillboardRestHttpClients()
            .AddGithubHttpClients();
    }

    private static IServiceCollection AddEveRestHttpClients(this IServiceCollection builder)
    {
        builder.AddHttpClient<IEveRestDataAccess, EveRestDataAccess>(AddDefaultRequestHeaders).ConfigurePrimaryHttpMessageHandler(() => new HttpClientHandler
        {
            MaxConnectionsPerServer = 100
        });
        ;
        return builder;
    }

    private static IServiceCollection AddZKillboardRestHttpClients(this IServiceCollection builder)
    {
        builder.AddHttpClient<IZKillboardRestDataAccess, ZKillboardRestDataAccess>(AddDefaultRequestHeaders);
        return builder;
    }

    private static IServiceCollection AddGithubHttpClients(this IServiceCollection builder)
    {
        builder.AddHttpClient<IGithubReleaseDataAccess, GithubReleaseDataAccess>(AddDefaultRequestHeaders);
        return builder;
    }

    #endregion

    #region helper methods

    private static Type? GetMatchingInterface(Type type) => type.GetInterfaces()
        .FirstOrDefault(t => t.Name == "I" + type.Name);

    private static void AddDefaultRequestHeaders(HttpClient httpClient)
    {
        httpClient.DefaultRequestHeaders.TryAddWithoutValidation("User-Agent", AppConstants.UserAgentHeader);
        httpClient.DefaultRequestHeaders.TryAddWithoutValidation("Content-Encoding", "gzip");
        httpClient.DefaultRequestHeaders.TryAddWithoutValidation("Content-Type", "application/json");
    }

    #endregion
}