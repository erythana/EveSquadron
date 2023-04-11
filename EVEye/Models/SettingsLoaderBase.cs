using System;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;

namespace EVEye.Models;

public abstract class SettingsLoaderBase
{
    private readonly IConfiguration _configuration;
    private readonly ILogger<SettingsLoaderBase> _logger;

    protected SettingsLoaderBase(IConfiguration configuration, ILogger<SettingsLoaderBase> logger)
    {
        _configuration = configuration;
        _logger = logger;
    }

    protected string LoadSetting(string settingName, bool required = true)
    {
        var value = _configuration.GetValue<string>($"{settingName}");
        if (required && string.IsNullOrWhiteSpace(value))
        {
            var error = $"Can not load required setting '{settingName}'";
            _logger.LogCritical(error);
            throw new InvalidOperationException(error);
        }

        _logger.LogDebug($"Loaded {(required ? "" : "(optional)")} setting for {settingName}, value: '{value}'");
        return value ?? string.Empty;
    }
}