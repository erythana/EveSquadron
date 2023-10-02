using System.Collections.Generic;
using System.Threading.Tasks;
using EveSquadron.DataAccess.Interfaces;
using EveSquadron.DataRepositories.Interfaces;
using EveSquadron.Models.Options;
using EveSquadron.Models.ZKillboard.Data;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

namespace EveSquadron.DataRepositories;

public class ZKillboardDataRepository : IZKillboardDataRepository
{
    #region member fields

    private readonly IZKillboardRestDataAccess _zKillboardRestDataAccess;
    private readonly ILogger<ZKillboardDataRepository> _logger;
    private readonly string _characterStatsEndpoint;
    private readonly string _characterEndpoint;

    #endregion
    
    #region constructor

    public ZKillboardDataRepository(IZKillboardRestDataAccess zKillboardRestDataAccess, IOptions<ZkillboardEndpointOptions> endpointOptions, ILogger<ZKillboardDataRepository> logger)
    {
        _zKillboardRestDataAccess = zKillboardRestDataAccess;
        _logger = logger;

        var options = endpointOptions.Value;
        _characterEndpoint = options.CharacterEndpoint;
        _characterStatsEndpoint = options.CharacterStatsEndpoint;
    }

    #endregion

    #region interface implementation

    public Task<ZKillboardCharacterStatistic> GetStatisticsFrom(int playerID)
    {
        return _zKillboardRestDataAccess.GetCharacterStatisticsAsync<ZKillboardCharacterStatistic>(_characterStatsEndpoint, playerID);
    }

    public Task<IEnumerable<ZKillboardEntry>> GetKillboardHistoryFor(int playerID)
    {
        return _zKillboardRestDataAccess.GetKillboardHistoryFor<ZKillboardEntry>(_characterEndpoint, playerID);
    }

    #endregion

}