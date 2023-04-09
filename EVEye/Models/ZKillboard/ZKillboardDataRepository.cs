using System.Threading.Tasks;
using EVEye.DataAccess.Interfaces;
using EVEye.Models.Interfaces;
using EVEye.Models.ZKillboard.Data;
using EVEye.Models.ZKillboard.Interfaces;
using Microsoft.Extensions.Logging;

namespace EVEye.Models.ZKillboard
{
    public class ZKillboardDataRepository : IZKillboardDataRepository
    {
        #region member fields

        private readonly IZKillboardRestDataAccess _zKillboardRestDataAccess;
        private readonly ILogger<ZKillboardDataRepository> _logger;
        private readonly string _characterStatsEndpoint;
        
        #endregion

        #region constructor

        public ZKillboardDataRepository(IZKillboardRestDataAccess zKillboardRestDataAccess, IZKillboardEndpointsLoader endpoints, ILogger<ZKillboardDataRepository> logger)
        {
            _zKillboardRestDataAccess = zKillboardRestDataAccess;
            _logger = logger;
            
            _characterStatsEndpoint = endpoints.CharacterStatsEndpoint;
        }
        
        #endregion

        #region interface implementation

        public Task<ZKillboardCharacterStatistic?> GetStatisticsFrom(int playerID)
        {
            return _zKillboardRestDataAccess.GetCharacterStatisticsAsync<ZKillboardCharacterStatistic>(_characterStatsEndpoint, playerID);
        }
        
        #endregion
    }
}