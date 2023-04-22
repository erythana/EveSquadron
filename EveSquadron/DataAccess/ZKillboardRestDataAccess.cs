using System.Collections.Generic;
using System.Net.Http;
using System.Text.Json;
using System.Threading.Tasks;
using EveSquadron.DataAccess.Base;
using EveSquadron.DataAccess.Interfaces;
using EveSquadron.Models;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;

namespace EveSquadron.DataAccess;

public sealed class ZKillboardRestDataAccess : RestDataAccessBase, IZKillboardRestDataAccess
{
    #region member fields

    private readonly HttpClient _httpClient;
    private readonly ILogger<ZKillboardRestDataAccess> _logger;

    #endregion

    #region constructor

    public ZKillboardRestDataAccess(HttpClient httpClient,
        IConfiguration configuration,
        ILogger<ZKillboardRestDataAccess> logger) : base(httpClient, logger)
    {
        _httpClient = httpClient;
        _logger = logger;
    }

    #endregion

    #region interface implementation

    public Task<T> GetCharacterStatisticsAsync<T>(string characterStatsEndpoint, int playerID)
    {
        _logger.LogDebug($"Loading Character statistics from endpoint {characterStatsEndpoint} for ID {playerID}");
        return GetByIdAsync<T>(characterStatsEndpoint, playerID);
    }

    public Task<IEnumerable<T>> GetKillboardHistoryFor<T>(string characterEndpoint, int playerID)
    {
        _logger.LogDebug($"Loading Killboard history from endpoint {characterEndpoint} for ID {playerID}");
        return GetAllAsync<T>($"{characterEndpoint}{playerID}/");
    }

    #endregion
    
    private async Task<T> GetByIdAsync<T>(string endpointUrl, int id) //Same method as in RestDataAccessBase - zKillboards needs a trailing backslash
    {
        _logger.LogDebug($"Executing GetByIdAsync on endpoint {endpointUrl}{id}/");
        var content = await _httpClient.GetStringAsync($"{endpointUrl}{id}/");
        
        return JsonSerializer.Deserialize<T>(content, ApplicationConstants.AppDefaultSerializerOptions)!;
    }
}