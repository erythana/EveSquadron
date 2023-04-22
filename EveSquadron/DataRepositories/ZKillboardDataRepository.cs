using System.Collections.Generic;
using System.Threading.Tasks;
using EveSquadron.DataAccess.Interfaces;
using EveSquadron.DataRepositories.Interfaces;
using EveSquadron.Models.Interfaces;
using EveSquadron.Models.ZKillboard.Data;
using Microsoft.Extensions.Logging;

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

    public ZKillboardDataRepository(IZKillboardRestDataAccess zKillboardRestDataAccess, IZKillboardEndpointsLoader endpoints, ILogger<ZKillboardDataRepository> logger)
    {
        _zKillboardRestDataAccess = zKillboardRestDataAccess;
        _logger = logger;

        _characterEndpoint = endpoints.CharacterEndpoint;
        _characterStatsEndpoint = endpoints.CharacterStatsEndpoint;
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