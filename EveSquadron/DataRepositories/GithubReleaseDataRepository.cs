using System.Threading.Tasks;
using EveSquadron.DataAccess.Interfaces;
using EveSquadron.DataRepositories.Interfaces;
using EveSquadron.Models;
using EveSquadron.Models.Interfaces;
using EveSquadron.Models.Options;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

namespace EveSquadron.DataRepositories;

public class GithubReleaseDataRepository : IGithubReleaseDataRepository
{
    #region member fields 
    
    private readonly IGithubReleaseDataAccess _githubReleaseDataAccess;
    private readonly IOptions<ReleaseEndpointOptions> _releaseOptions;
    private readonly ILogger<GithubReleaseDataRepository> _logger;

    #endregion
    
    #region constructor
    
    public GithubReleaseDataRepository(IGithubReleaseDataAccess githubReleaseDataAccess, IOptions<ReleaseEndpointOptions> releaseOptions, ILogger<GithubReleaseDataRepository> logger)
    {
        _githubReleaseDataAccess = githubReleaseDataAccess;
        _releaseOptions = releaseOptions;
        _logger = logger;
    }
    
    #endregion
    
    #region interface implementation

    public Task<GithubReleaseInformation> GetLatestReleaseInformationFrom(string endpoint) => _githubReleaseDataAccess.GetLatestReleaseInformation<GithubReleaseInformation>(_releaseOptions.Value.ReleaseVersionAPIEndpoint);
    
    #endregion
}