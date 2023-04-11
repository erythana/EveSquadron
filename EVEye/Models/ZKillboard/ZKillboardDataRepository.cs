using System.Collections.Generic;
using System.Threading.Tasks;
using EVEye.DataAccess.Interfaces;
using EVEye.Models.Interfaces;
using EVEye.Models.ZKillboard.Data;
using EVEye.Models.ZKillboard.Interfaces;
using Microsoft.Extensions.Logging;

namespace EVEye.Models.ZKillboard;

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

    public Task<ZKillboardCharacterStatistic?> GetStatisticsFrom(int playerID) => _zKillboardRestDataAccess.GetCharacterStatisticsAsync<ZKillboardCharacterStatistic>(_characterStatsEndpoint, playerID);
    public Task<IEnumerable<ZKillboardHistory>?> GetKillboardHistoryFor(int playerID) => _zKillboardRestDataAccess.GetKillboardHistoryFor<ZKillboardHistory>(_characterEndpoint, playerID);

    #endregion

}