using System;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;

namespace EveSquadron.Models;

public abstract class SettingsLoaderBase
{
    #region member fields
    
    private readonly IConfiguration _configuration;
    private readonly ILogger<SettingsLoaderBase> _logger;

    #endregion
    
    #region constructor
    
    protected SettingsLoaderBase(IConfiguration configuration, ILogger<SettingsLoaderBase> logger)
    {
        _configuration = configuration;
        _logger = logger;
    }

    #endregion
    
    #region helper methods

    protected string LoadSetting(string settingName, bool required = true)
    {
        var value = _configuration.GetValue<string>($"{settingName}");
        if (required && string.IsNullOrWhiteSpace(value))
        {
            var error = $"Could not load required setting '{settingName}'";
            _logger.LogCritical(error);
            throw new InvalidOperationException(error);
        }

        _logger.LogInformation($"Loaded {(required ? "" : "(optional) ")}setting for '{settingName}', value: '{value}'");
        return value ?? string.Empty;
    }
    
    #endregion
}