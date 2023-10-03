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
using Microsoft.Extensions.Logging.EventLog;
using Microsoft.Extensions.Options;

namespace EveSquadron.DependencyInjection;

public static class ContainerConfiguration
{
    #region member fields

    private static readonly IConfiguration Configuration;

    #endregion

    #region constructor

    static ContainerConfiguration()
    {
        var configuration = new ConfigurationBuilder()
            .SetBasePath(Directory.GetCurrentDirectory())
            .AddJsonFile("appsettings.json")
            .AddJsonFile("appsettings.Logging.json")
            .Build();

        Configuration = new ConfigurationBuilder()
            .AddConfiguration(configuration)
            .Add(new SqLiteConfigurationSource(configuration))
            .Build();
    }

    #endregion

    #region Container Initialization/Configuration

    public static IServiceCollection Configure(this IServiceCollection builder) => builder
        .AddConfiguration()
        .AddOptionMappings()
        .AddLogging()
        .RegisterTransientsByConvention()
        .RegisterTransientsNonConvention()
        .RegisterSingletons()
        .RegisterHttpClients();


    private static IServiceCollection AddConfiguration(this IServiceCollection builder)
    {
        builder.AddSingleton(Configuration);
        return builder;
    }
    
    private static IServiceCollection AddOptionMappings(this IServiceCollection builder)
    {
        AddOptionAndValidateDataAnnotations<EveSquadronOptions>(Configuration.GetSection(EveSquadronOptions.Section));
        AddOptionAndValidateDataAnnotations<StatusOptions>(Configuration.GetSection(StatusOptions.Section));
        AddOptionAndValidateDataAnnotations<ReleaseEndpointOptions>(Configuration.GetSection(ReleaseEndpointOptions.Section));
        AddOptionAndValidateDataAnnotations<ZkillboardEndpointOptions>(Configuration.GetSection(ZkillboardEndpointOptions.Section));
        AddOptionAndValidateDataAnnotations<EveEndpointOptions>(Configuration.GetSection(EveEndpointOptions.Section));
        return builder;

        void AddOptionAndValidateDataAnnotations<T>(IConfiguration configuration) where T : class => builder.AddOptions<T>()
                .Bind(configuration)
                .ValidateDataAnnotations();
    }

    private static IServiceCollection AddLogging(this IServiceCollection builder)
    {
        var loggingConfiguration = Configuration.GetSection("Logging");

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
        .AddSingleton<ViewLocator>()
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