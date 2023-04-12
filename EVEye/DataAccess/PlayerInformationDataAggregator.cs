using System;
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

    //TODO CLEAN UP AND TRY TO LIMIT QUERIES
    public async Task<IEnumerable<EVEyePlayerInformation>> GetAggregatedItemsFor(IEnumerable<string> players)
    {
        var results = new List<AggregatorInformation>();
        var eveNameIDMappings = await _eveDataRepository.GetIDsFrom(players);

        //First query the data for each character and also query any single-value endpoints
        foreach (var character in eveNameIDMappings.Characters)
        {
            var charInfo = _eveDataRepository.GetCharacterInformationFor(character.ID);
            var statistics = _zKillboardDataRepository.GetStatisticsFrom(character.ID);
            var zKillboardHistory = _zKillboardDataRepository.GetKillboardHistoryFor(character.ID);//zKillboard returns 200 values
            
            var currentPlayerInformation = await Task.WhenAll(charInfo, statistics, zKillboardHistory).ContinueWith(t =>
            {
                if (t.Status != TaskStatus.RanToCompletion)
                    throw new InvalidOperationException($"Could not fetch information from Servers: {t.Exception}");

                return new EVEyePlayerInformation
                {
                    ID = character.ID,
                    CharacterName = character.Name,
                    Birthday = DateTime.Parse(charInfo.Result.Birthday),
                    CharacterImage = _eveDataRepository.GetPortraitFrom(character.ID, 32),
                    SecurityStanding = charInfo.Result.SecurityStatus,
                };
            });

            var aggregatorInformation = new AggregatorInformation()
            {
                EVEyePlayerInformation = currentPlayerInformation,
                ZKillboardCharacterStatistic = statistics.Result,
                KillboardHistory = zKillboardHistory.Result
            };
            
            results.Add(aggregatorInformation);
        }

        await BulkUpdatePlayerInformation(results);

        return results.Select(x => x.EVEyePlayerInformation);
    }

    #endregion

    private async Task BulkUpdatePlayerInformation(IEnumerable<AggregatorInformation> results)
    {
        //now we can bulk query every eve item and set it
        var idsToLookup = GetLookupIDsFrom(results);
        var lookup = (await _eveDataRepository.GetNamesFrom(idsToLookup)).ToList();

        foreach (var player in results)
        {
            player.EVEyePlayerInformation.CorporationName = lookup.FirstOrDefault(c => c.ID == player.ZKillboardCharacterStatistic.Info?.CorporationID)?.Name!;
            player.EVEyePlayerInformation.AllianceName = lookup.FirstOrDefault(a => a.ID == player.ZKillboardCharacterStatistic.Info?.AllianceID)?.Name;
            player.EVEyePlayerInformation.PlayerDetails = GetEVEyePlayerDetails(player);
        }
    }
    
    private EVEyePlayerDetails GetEVEyePlayerDetails(AggregatorInformation player)
    {
        var latestKillboardActivity = GetFirstDetailedKillboardInfoAsync(player);

        return new EVEyePlayerDetails
        {
            ID = player.EVEyePlayerInformation.ID,
            DangerRatio = player.ZKillboardCharacterStatistic.DangerRatio,
            GangRatio = player.ZKillboardCharacterStatistic.GangRatio,
            ShipsDestroyed = player.ZKillboardCharacterStatistic.ShipsDestroyed,
            ShipsLost = player.ZKillboardCharacterStatistic.ShipsLost,
            SoloKills = player.ZKillboardCharacterStatistic.SoloKills,
            SoloLosses = player.ZKillboardCharacterStatistic.SoloLosses,
            LatestKillboardActivity = GetEveyeKillInformation(latestKillboardActivity)
        };
    }

    private Task<EVEyeKillInformation>? GetEveyeKillInformation(Task<EveDetailedKillInformation?>? latestKillboardActivity)
    {
        return latestKillboardActivity?.ContinueWith<EVEyeKillInformation>(t =>
        {
            if (t.Status != TaskStatus.RanToCompletion)
                throw new InvalidOperationException("Could not fetch information from Servers");
            
            return new EVEyeKillInformation()
            {
                Date = t.Result!.KillDate,
                VictimShip = _eveDataRepository.GetNamesFrom(new []{t.Result!.Victim.ShipTypeID}).ContinueWith(list =>
                {
                    if (list.Status != TaskStatus.RanToCompletion)
                        throw new InvalidOperationException("Could not fetch information from Servers");
                    return list.Result.First().Name;
                }),
                AttackerShip = _eveDataRepository.GetNamesFrom(new []{Enumerable.First<EveKillAttacker>(t!.Result.Attackers, x => x.FinalBlow).ShipTypeID}).ContinueWith(list =>
                {
                    if (list.Status != TaskStatus.RanToCompletion)
                        throw new InvalidOperationException("Could not fetch information from Servers");
                    return list.Result.First().Name;
                }),
                AttackerGuns = _eveDataRepository.GetNamesFrom(new []{Enumerable.First<EveKillAttacker>(t!.Result.Attackers, x => x.FinalBlow).WeaponTypeID}).ContinueWith(list =>
                {
                    if (list.Status != TaskStatus.RanToCompletion)
                        throw new InvalidOperationException("Could not fetch information from Servers");
                    return list.Result.First().Name;
                }),
                SolarSystem = _eveDataRepository.GetNamesFrom(new []{t!.Result.SolarSystemID}).ContinueWith(list =>
                {
                    if (list.Status != TaskStatus.RanToCompletion)
                        throw new InvalidOperationException("Could not fetch information from Servers");
                    return list.Result.First().Name;
                }),
            };
        });
    }

    private Task<EveDetailedKillInformation?>? GetFirstDetailedKillboardInfoAsync(AggregatorInformation player, Func<ZKillboardEntry, bool>? killboardLookup = null)
    {
        Task<EveDetailedKillInformation?>? detailedKillInfo = null;
        var latestKillboardActivity = player.KillboardHistory?.FirstOrDefault(killboardLookup ?? (_ => true));
        if (latestKillboardActivity is not null)
            detailedKillInfo = _eveDataRepository.GetDetailedKillInformation(latestKillboardActivity.ID, latestKillboardActivity.ZKillboardKill.Hash);
        
        return detailedKillInfo;
    }

    private IEnumerable<int> GetLookupIDsFrom(IEnumerable<AggregatorInformation> results)
    {
        return Enumerable.Empty<int>()
            .Concatenate(
                results
                    .Select(kvp => kvp.ZKillboardCharacterStatistic.Info?.CorporationID ?? 0)
                    .Where(id => id > 0),
                results
                    .Select(kvp => kvp.ZKillboardCharacterStatistic.Info?.AllianceID ?? 0)
                    .Where(id => id > 0));
    }
}