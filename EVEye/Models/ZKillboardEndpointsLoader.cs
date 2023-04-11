using EVEye.Models.Interfaces;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;

namespace EVEye.Models;

public class ZKillboardEndpointsLoader : SettingsLoaderBase, IZKillboardEndpointsLoader
{

    #region constructor

    public ZKillboardEndpointsLoader(IConfiguration configuration, ILogger<ZKillboardEndpointsLoader> logger) : base(configuration, logger)
    {
        _configuration = configuration;
        _logger = logger;

        var zKillboardEndpointPath = "Endpoints:ZKillboard";
        CharacterEndpoint = LoadSetting($"{zKillboardEndpointPath}:CharacterEndpoint");
        CharacterStatsEndpoint = LoadSetting($"{zKillboardEndpointPath}:CharacterStatsEndpoint");
    }

    #endregion

    #region properties

    public string CharacterEndpoint { get; }
    public string CharacterStatsEndpoint { get; }

    #endregion
    #region member fields

    private readonly IConfiguration _configuration;
    private readonly ILogger<ZKillboardEndpointsLoader> _logger;

    #endregion
}