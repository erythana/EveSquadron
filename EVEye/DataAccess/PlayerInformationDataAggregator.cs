using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using EVEye.DataAccess.Base;
using EVEye.DataAccess.Interfaces;
using EVEye.Extensions;
using EVEye.Models.EVE.Interfaces;
using EVEye.Models.EVEye;
using EVEye.Models.ZKillboard.Data;
using EVEye.Models.ZKillboard.Interfaces;
using Microsoft.Extensions.Logging;

namespace EVEye.DataAccess
{
    public class PlayerInformationDataAggregator : IPlayerInformationDataAggregator
    {
        #region member fields
        
        private readonly IEveDataRepository _eveDataRepository;
        private readonly IZKillboardDataRepository _zKillboardDataRepository;
        private readonly ILogger<PlayerInformationDataAggregator> _logger;
        
        #endregion

        #region constructor

        public PlayerInformationDataAggregator(IEveDataRepository eveDataRepository, IZKillboardDataRepository zKillboardDataRepository, ILogger<PlayerInformationDataAggregator> logger)
        {
            _eveDataRepository = eveDataRepository;
            _zKillboardDataRepository = zKillboardDataRepository;
            _logger = logger;
        }

        #endregion

        #region interface implementation

        public async Task<IEnumerable<EVEyePlayerInformation>> GetAggregatedItemsFor(IEnumerable<string> players)
        {
            var results = new List<(EVEyePlayerInformation playerInformation, ZKillboardCharacterStatistic characterStatistic)>();
            var eveNameIDMappings = await _eveDataRepository.GetIDsFrom(players);

            //First query the data for each character and also query any single-value endpoints
            foreach (var character in eveNameIDMappings.Characters)
            {
                var portrait = _eveDataRepository.GetPortraitFrom(character.ID!.Value, 32);
                var statistics = _zKillboardDataRepository.GetStatisticsFrom(character.ID!.Value);

                var currentPlayerInformation = await Task.WhenAll(portrait, statistics).ContinueWith(_ => new EVEyePlayerInformation()
                {
                    ID = character.ID,
                    CharacterName = character.Name,
                    CharacterImage = portrait.Result,
                    SecurityStanding = statistics.Result!.Info.SecStatus,
                });
                results.Add((currentPlayerInformation, statistics.Result!));
            }
            await BulkUpdatePlayerInformation(results);
            
            return results.Select(x => x.playerInformation);
        }

        #endregion

        private async Task BulkUpdatePlayerInformation(List<(EVEyePlayerInformation playerInformation, ZKillboardCharacterStatistic characterStatistic)> results)
        {
            //now we can bulk query every eve item and set it
            var idsToLookup = Enumerable.Empty<int>()
                .Concatenate(
                    results
                        .Select(kvp => kvp.characterStatistic.Info.CorporationID)
                        .Where(id => id > 0),
                    results
                        .Select(kvp => kvp.characterStatistic.Info.AllianceID)
                        .Where(id => id > 0));

            var lookup = await _eveDataRepository.GetNamesFrom(idsToLookup);
            foreach (var player in results)
            {
                player.playerInformation.CorporationName = lookup?.FirstOrDefault(c => c.ID == player.characterStatistic.Info.CorporationID)?.Name;
                player.playerInformation.AllianceName = lookup?.FirstOrDefault(a => a.ID == player.characterStatistic.Info.AllianceID)?.Name;
                player.playerInformation.PlayerDetails = new EVEyePlayerDetails()
                {
                    ID = player.playerInformation.ID!.Value,
                    DangerRatio = player.characterStatistic.DangerRatio,
                    GangRatio = player.characterStatistic.GangRatio,
                    ShipsDestroyed = player.characterStatistic.ShipsDestroyed,
                    ShipsLost = player.characterStatistic.ShipsLost,
                    SoloKills = player.characterStatistic.SoloKills,
                    SoloLosses = player.characterStatistic.SoloLosses,
                };
            }
        }
    }

}