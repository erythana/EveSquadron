using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using EVEye.DataAccess.Base;
using EVEye.DataAccess.Interfaces;
using EVEye.Models;
using EVEye.Models.EVE.Interfaces;
using EVEye.Models.ZKillboard.Interfaces;
using Microsoft.Extensions.Logging;

namespace EVEye.DataAccess
{
    public class PlayerInformationDataAggregator : DataAggregatorBase<EVEyePlayerInformation, IEnumerable<string>>, IPlayerInformationDataAggregator
    {
        private readonly IEveDataRepository _eveDataRepository;
        private readonly IZKillboardDataRepository _zKillboardDataRepository;
        private readonly ILogger<PlayerInformationDataAggregator> _logger;

        public PlayerInformationDataAggregator(IEveDataRepository eveDataRepository, IZKillboardDataRepository zKillboardDataRepository, ILogger<PlayerInformationDataAggregator> logger)
        {
            _eveDataRepository = eveDataRepository;
            _zKillboardDataRepository = zKillboardDataRepository;
            _logger = logger;
        }
        
        protected async override Task<IEnumerable<EVEyePlayerInformation>> GetAggregatedItems(IEnumerable<string> players)
        {
            var eveNameIDMappings = await _eveDataRepository.GetIDsFrom(players);

            //TODO: REMOVE, THIS IS JUST A TEST
            return await Task.WhenAll(eveNameIDMappings.Characters.Select(async x => new EVEyePlayerInformation()
            {
                CharacterImage = await _eveDataRepository.GetPortraitFrom(x.ID, 32),
                CharacterName = x.Name
            }));
          
        }

        public Task<IEnumerable<EVEyePlayerInformation>> GetAggregatedItemsFor(IEnumerable<string> players) => GetAggregatedItems(players);
    }

}