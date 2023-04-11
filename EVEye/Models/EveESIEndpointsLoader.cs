using EVEye.Models.Interfaces;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;

namespace EVEye.Models;

public class EveESIEndpointsLoader : SettingsLoaderBase, IEveESIEndpointsLoader
{
    #region constructor

    public EveESIEndpointsLoader(IConfiguration configuration, ILogger<EveESIEndpointsLoader> logger) : base(configuration, logger)
    {
        _configuration = configuration;
        var esiEndpointPath = "Endpoints:EveESI";

        _logger = logger;
        UniverseEndpoint = LoadSetting($"{esiEndpointPath}:UniverseEndpoint");
        KillmailEndpoint = LoadSetting($"{esiEndpointPath}:KillmailEndpoint");
        PortraitEndpoint = LoadSetting($"{esiEndpointPath}:PortraitEndpoint");
    }

    #endregion
    #region member fields

    private readonly IConfiguration _configuration;
    private readonly ILogger<EveESIEndpointsLoader> _logger;

    #endregion

    #region properties

    public string UniverseEndpoint { get; }
    public string PortraitEndpoint { get; }
    public string KillmailEndpoint { get; }

    #endregion
}