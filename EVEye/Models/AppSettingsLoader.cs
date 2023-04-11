using EVEye.Models.Interfaces;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;

namespace EVEye.Models;

public class AppSettingsLoader : SettingsLoaderBase, IAppSettingsLoader
{
    #region constructor

    public AppSettingsLoader(IConfiguration configuration, ILogger<AppSettingsLoader> logger) : base(configuration, logger)
    {
        _configuration = configuration;

        _logger = logger;
        ClipboardPollingMilliseconds = LoadSetting("ClipboardPollingMilliseconds", false);
        Theme = LoadSetting("Theme", false);
    }

    #endregion
    #region member fields

    private readonly IConfiguration _configuration;
    private readonly ILogger<AppSettingsLoader> _logger;

    #endregion

    #region properties

    public string ClipboardPollingMilliseconds { get; set; }

    public string Theme { get; set; }

    #endregion
}