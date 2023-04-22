using EveSquadron.Models.Interfaces;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;

namespace EveSquadron.Models;

public class ReleaseSettingsLoader : SettingsLoaderBase, IReleaseSettingsLoader
{
    #region member fields

    private readonly IConfiguration _configuration;
    private readonly ILogger<ReleaseSettingsLoader> _logger;

    #endregion

    #region constructor

    public ReleaseSettingsLoader(IConfiguration configuration, ILogger<ReleaseSettingsLoader> logger) : base(configuration, logger)
    {
        _configuration = configuration;
        _logger = logger;

        ReleasePath = LoadSetting($"Endpoints:Release:ReleasePath");
        ReleaseVersionAPIEndpoint = LoadSetting($"Endpoints:Release:ReleaseVersionAPIEndpoint");
    }

    #endregion

    #region properties

    public string ReleasePath { get; }
    public string ReleaseVersionAPIEndpoint { get; }

    #endregion
}