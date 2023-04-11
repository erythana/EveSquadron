using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using EVEye.DataAccess.Interfaces;
using EVEye.Extensions;
using EVEye.Models.EVE.Data;
using EVEye.Models.EVE.Interfaces;
using EVEye.Models.EVEye;
using EVEye.Models.ZKillboard.Data;
using EVEye.Models.ZKillboard.Interfaces;
using Microsoft.Extensions.Logging;

namespace EVEye.DataAccess;

public class PlayerInformationDataAggregator : IPlayerInformationDataAggregator
{

    #region constructor

    public PlayerInformationDataAggregator(IEveDataRepository eveDataRepository, IZKillboardDataRepository zKillboardDataRepository, ILogger<PlayerInformationDataAggregator> logger)
    {
        _eveDataRepository = eveDataRepository;
        _zKillboardDataRepository = zKillboardDataRepository;
        _logger = logger;
    }

    #endregion

    #region interface implementation

    //TODO CLEAN UP AND TRY TO LIMIT QUERIES
    public async Task<IEnumerable<EVEyePlayerInformation>> GetAggregatedItemsFor(IEnumerable<string> players)
    {
        var results = new List<(EVEyePlayerInformation playerInformation, ZKillboardCharacterStatistic characterStatistic, List<EveDetailedKillInformation> detailedKillInformationList)>();
        var eveNameIDMappings = await _eveDataRepository.GetIDsFrom(players);

        //First query the data for each character and also query any single-value endpoints
        foreach (var character in eveNameIDMappings.Characters)
        {
            var portrait = _eveDataRepository.GetPortraitFrom(character.ID!.Value, 32);
            var statistics = _zKillboardDataRepository.GetStatisticsFrom(character.ID!.Value);
            var zKillboardHistory = _zKillboardDataRepository.GetKillboardHistoryFor(character.ID!.Value);//zKillboard returns 200 values

            var currentPlayerInformation = await Task.WhenAll(portrait, statistics, zKillboardHistory).ContinueWith(_ => new EVEyePlayerInformation
            {
                ID = character.ID,
                CharacterName = character.Name,
                CharacterImage = portrait.Result,
                SecurityStanding = statistics.Result?.Info?.SecStatus
            });

            var detailedKillList = new List<EveDetailedKillInformation>();


            //THIS IS SLOW!
            // var historyQuery = zKillboardHistory.Result.Select(h => _eveDataRepository.GetDetailedKillInformation(h.ID, h.ZKillboardEntry.Hash));
            // var resultSet = Task.WhenAll(historyQuery);
            // foreach (var killboardEntry in zKillboardHistory.Result!)
            // {
            //     var currentDetailedKillInformation = await _eveDataRepository.GetDetailedKillInformation(killboardEntry.ID, killboardEntry.ZKillboardEntry.Hash);
            //     detailedKillList.Add(currentDetailedKillInformation);
            // }

            results.Add((currentPlayerInformation, statistics.Result!, detailedKillList));
        }

        await BulkUpdatePlayerInformation(results);

        return results.Select(x => x.playerInformation);
    }

    #endregion

    private async Task BulkUpdatePlayerInformation(
        List<(EVEyePlayerInformation playerInformation, ZKillboardCharacterStatistic characterStatistic, List<EveDetailedKillInformation> detailedKillInformations)> results)
    {
        //now we can bulk query every eve item and set it
        var idsToLookup = Enumerable.Empty<int>()
            .Concatenate(
                results
                    .Select(kvp => kvp.characterStatistic.Info?.CorporationID ?? 0)
                    .Where(id => id > 0),
                results
                    .Select(kvp => kvp.characterStatistic.Info?.AllianceID ?? 0)
                    .Where(id => id > 0));

        var lookup = await _eveDataRepository.GetNamesFrom(idsToLookup);
        foreach (var player in results)
        {
            player.playerInformation.CorporationName = lookup?.FirstOrDefault(c => c.ID == player.characterStatistic.Info?.CorporationID)?.Name;
            player.playerInformation.AllianceName = lookup?.FirstOrDefault(a => a.ID == player.characterStatistic.Info?.AllianceID)?.Name;
            player.playerInformation.PlayerDetails = new EVEyePlayerDetails
            {
                ID = player.playerInformation.ID!.Value,
                DangerRatio = player.characterStatistic.DangerRatio,
                GangRatio = player.characterStatistic.GangRatio,
                ShipsDestroyed = player.characterStatistic.ShipsDestroyed,
                ShipsLost = player.characterStatistic.ShipsLost,
                SoloKills = player.characterStatistic.SoloKills,
                SoloLosses = player.characterStatistic.SoloLosses
            };
        }
    }

    #region member fields

    private readonly IEveDataRepository _eveDataRepository;
    private readonly IZKillboardDataRepository _zKillboardDataRepository;
    private readonly ILogger<PlayerInformationDataAggregator> _logger;

    #endregion
}