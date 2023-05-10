using EveSquadron.Models.Interfaces;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;

namespace EveSquadron.Models;

public class AppSettingsLoader : SettingsLoaderBase, IAppSettingsLoader
{
    #region member fields

    private readonly IConfiguration _configuration;
    private readonly ILogger<AppSettingsLoader> _logger;

    #endregion
    
    #region constructor

    public AppSettingsLoader(IConfiguration configuration, ILogger<AppSettingsLoader> logger) : base(configuration, logger)
    {
        _configuration = configuration;

        _logger = logger;
        ClipboardPollingMilliseconds = LoadSetting("ClipboardPollingMilliseconds", false);
        Theme = LoadSetting("Theme", false);
        HoverColor = LoadSetting("HoverColor", false);
        ShowPortrait = LoadSetting("ShowPortrait", false);
        GridRowSize = LoadSetting("GridRowSize", false);
    }

    #endregion

    #region properties

    public string ClipboardPollingMilliseconds { get; set; }

    public string Theme { get; set; }
    
    public string HoverColor { get; set; }
    
    public string ShowPortrait { get; set; }
    
    public string GridRowSize { get; set; }

    #endregion
}