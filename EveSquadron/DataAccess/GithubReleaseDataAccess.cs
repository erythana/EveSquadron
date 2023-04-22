using System.Net.Http;
using System.Threading.Tasks;
using EveSquadron.DataAccess.Base;
using EveSquadron.DataAccess.Interfaces;
using Microsoft.Extensions.Logging;

namespace EveSquadron.DataAccess;

public class GithubReleaseDataAccess : RestDataAccessBase, IGithubReleaseDataAccess
{
    #region member fields 
    
    private readonly HttpClient _httpClient;
    private readonly ILogger<GithubReleaseDataAccess> _logger;

    #endregion
    
    #region constructor
    
    public GithubReleaseDataAccess(HttpClient httpClient, ILogger<GithubReleaseDataAccess> logger) : base(httpClient, logger)
    {
        _httpClient = httpClient;
        _logger = logger;
    }
    
    #endregion
    
    #region interface implementation
    
    public Task<T> GetLatestReleaseInformation<T>(string endpoint) => GetByIdAsync<T>(endpoint);
    
    #endregion
}