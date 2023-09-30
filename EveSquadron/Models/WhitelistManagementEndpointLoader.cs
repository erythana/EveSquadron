using System;
using System.IO;
using EveSquadron.Models.Interfaces;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;

namespace EveSquadron.Models;

public class WhitelistManagementEndpointLoader : AppSettingsLoader, IWhitelistManagementEndpointLoader
{
    #region member fields

    private readonly IConfiguration _configuration;
    private readonly ILogger<AppSettingsLoader> _logger;

    #endregion
    #region constructor

    public WhitelistManagementEndpointLoader(IConfiguration configuration, ILogger<AppSettingsLoader> logger) : base(configuration, logger)
    {
        _configuration = configuration;
        _logger = logger;

        WhitelistManagementEndpoint = LoadSetting($"Endpoints:Whitelist:WhitelistDatabasePath", false);

        if (!string.IsNullOrWhiteSpace(WhitelistManagementEndpoint))
            return;
        
        WhitelistManagementEndpoint = Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData);
        _logger.LogInformation($"Setting {nameof(WhitelistManagementEndpoint)} to '{WhitelistManagementEndpoint}'");
    }

    #endregion

    #region properties

    public string WhitelistManagementEndpoint { get; }

    #endregion
}
