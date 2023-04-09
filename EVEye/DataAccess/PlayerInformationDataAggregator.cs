using System.Collections.Generic;
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
            var results = new List<EVEyePlayerInformation>();
            var eveNameIDMappings = await _eveDataRepository.GetIDsFrom(players);
            foreach (var characterNameID in eveNameIDMappings.Characters)
            {
                var portrait = _eveDataRepository.GetPortraitFrom(characterNameID.ID, 32);
                var statistics = _zKillboardDataRepository.GetStatisticsFrom(characterNameID.ID);
                
                var currentPlayerInformation = await Task.WhenAll(portrait, statistics).ContinueWith(t => new EVEyePlayerInformation()
                {
                    ID = characterNameID.ID,
                    CharacterName = characterNameID.Name,
                    CharacterImage = portrait.Result,
                    CorporationName = statistics.Result.Info.CorporationID.ToString(),//TODO
                    AllianceName = statistics.Result.Info.AllianceID.ToString(),//TODO
                    SecurityStanding = statistics.Result.Info.SecStatus
                });
                results.Add(currentPlayerInformation);
            }

            return results;
        }

        public Task<IEnumerable<EVEyePlayerInformation>> GetAggregatedItemsFor(IEnumerable<string> players) => GetAggregatedItems(players);
    }

}