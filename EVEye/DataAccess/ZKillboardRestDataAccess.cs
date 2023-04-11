using System.Collections.Generic;
using System.Net.Http;
using System.Threading.Tasks;
using EVEye.DataAccess.Base;
using EVEye.DataAccess.Interfaces;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;

namespace EVEye.DataAccess;

public sealed class ZKillboardRestDataAccess : RestDataAccessBase, IZKillboardRestDataAccess
{
    #region member fields

    private readonly ILogger<ZKillboardRestDataAccess> _logger;

    #endregion

    #region constructor

    public ZKillboardRestDataAccess(HttpClient httpClient,
        IConfiguration configuration,
        ILogger<ZKillboardRestDataAccess> logger) : base(httpClient, logger)
    {
        _logger = logger;
    }

    #endregion

    #region interface implementation

    public Task<T?> GetCharacterStatisticsAsync<T>(string characterStatsEndpoint, int playerID)
    {
        _logger.LogDebug($"Loading Character statistics from endpoint {characterStatsEndpoint} for ID {playerID}");
        return GetByIdAsync<T>(characterStatsEndpoint, playerID);
    }

    public async Task<IEnumerable<T>?> GetKillboardHistoryFor<T>(string characterEndpoint, int playerID)
    {
        _logger.LogDebug($"Loading Killboard history from endpoint {characterEndpoint} for ID {playerID}");

        return await GetAllAsync<T>($"{characterEndpoint}{playerID}/");
    }

    #endregion
}