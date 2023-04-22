using System.Threading.Tasks;
using EveSquadron.DataAccess.Interfaces;
using EveSquadron.DataRepositories.Interfaces;
using EveSquadron.Models;
using EveSquadron.Models.Interfaces;
using Microsoft.Extensions.Logging;

namespace EveSquadron.DataRepositories;

public class GithubReleaseDataRepository : IGithubReleaseDataRepository
{
    #region member fields 
    
    private readonly IGithubReleaseDataAccess _githubReleaseDataAccess;
    private readonly IReleaseSettingsLoader _releaseSettingsLoader;
    private readonly ILogger<GithubReleaseDataRepository> _logger;

    #endregion
    
    #region constructor
    
    public GithubReleaseDataRepository(IGithubReleaseDataAccess githubReleaseDataAccess, IReleaseSettingsLoader releaseSettingsLoader, ILogger<GithubReleaseDataRepository> logger)
    {
        _githubReleaseDataAccess = githubReleaseDataAccess;
        _releaseSettingsLoader = releaseSettingsLoader;
        _logger = logger;
    }
    
    #endregion
    
    #region interface implementation

    public Task<GithubReleaseInformation> GetLatestReleaseInformationFrom(string endpoint) => _githubReleaseDataAccess.GetLatestReleaseInformation<GithubReleaseInformation>(_releaseSettingsLoader.ReleaseVersionAPIEndpoint);
    
    #endregion
}