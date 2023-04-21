using EveSquadron.Models.Interfaces;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;

namespace EveSquadron.Models;

public class ZKillboardEndpointsLoader : SettingsLoaderBase, IZKillboardEndpointsLoader
{
    #region member fields

    private readonly IConfiguration _configuration;
    private readonly ILogger<ZKillboardEndpointsLoader> _logger;

    #endregion
    
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
}