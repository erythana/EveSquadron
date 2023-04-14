using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using EVEye.DataAccess.Interfaces;
using EVEye.Models;
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
    public async IAsyncEnumerable<EVEyePlayerInformation> GetAggregatedItemsFor(IEnumerable<string> players)
    {
        var eveNameIDMappings = await _eveDataRepository.GetIDsFrom(players);

        foreach (var character in eveNameIDMappings.Characters)
        {
            var currentPlayer = new EVEyePlayerInformation()
            {
                ID = character.ID,
                CharacterName = character.Name,
                CharacterImage = _eveDataRepository.GetPortraitFrom(character.ID, 32),
            };
            //fire and forget - these and subsequent tasks may fail!
            currentPlayer.PlayerDetails = GetLazyPlayerDetails(character);
            SetCharacterInfo(currentPlayer);

            yield return currentPlayer;
        }
    }

    private async Task SetCharacterInfo(EVEyePlayerInformation currentPlayer)
    {
        await _eveDataRepository.GetCharacterInformationFor(currentPlayer.ID).ContinueWith(async charInfoTask =>
        {
            if (charInfoTask.Status == TaskStatus.RanToCompletion)
            {
                currentPlayer.SecurityStanding = charInfoTask.Result.SecurityStatus;
                currentPlayer.Birthday = DateTime.Parse(charInfoTask.Result.Birthday);
                await _eveDataRepository.GetNamesFrom(GetValidIDs(charInfoTask.Result.CorporationID, charInfoTask.Result.AllianceID))
                    .ContinueWith(nameLookupTask =>
                    {
                        if (nameLookupTask.Status == TaskStatus.RanToCompletion)
                        {
                            currentPlayer.CorporationName = nameLookupTask.Result.FirstOrDefault(c => c.ID == charInfoTask.Result.CorporationID)?.Name;
                            currentPlayer.AllianceName = nameLookupTask.Result.FirstOrDefault(a => a.ID == charInfoTask.Result.AllianceID)?.Name;
                            return Task.CompletedTask;
                        }

                        _logger.LogError(nameLookupTask.Exception, "Could not fetch information from Servers");
                        throw new InvalidOperationException("Could not fetch information from Servers");
                    });
                return Task.CompletedTask;
            }

            _logger.LogError(charInfoTask.Exception, "Could not fetch information from Servers");
            throw new InvalidOperationException("Could not fetch information from Servers");
        });
    }

    private Lazy<Task<EVEyePlayerDetails>> GetLazyPlayerDetails(EveNameIDMapping character) =>
        new(() => Task.Run(async () =>
        {
            try
            {
                //Run these in sequence, zKillboard Rate Limit is harsh
                var zKillHistory = _zKillboardDataRepository.GetKillboardHistoryFor(character.ID);
                await zKillHistory;
                await Task.Delay(ApplicationConstants.ZKillboardAPILimits.ZKillboardRateLimitMs);
                var zKillStatistic = _zKillboardDataRepository.GetStatisticsFrom(character.ID);
                await zKillStatistic;
                if (zKillHistory.Status == TaskStatus.RanToCompletion && zKillStatistic.Status == TaskStatus.RanToCompletion)
                {
                    return new EVEyePlayerDetails()
                    {
                        ID = character.ID,
                        DangerRatio = zKillStatistic.Result.DangerRatio,
                        GangRatio = zKillStatistic.Result.GangRatio,
                        ShipsDestroyed = zKillStatistic.Result.ShipsDestroyed,
                        ShipsLost = zKillStatistic.Result.ShipsLost,
                        SoloKills = zKillStatistic.Result.SoloKills,
                        SoloLosses = zKillStatistic.Result.SoloLosses,
                        LatestKillboardActivity = GetEVEyeKillInformation(GetFirstDetailedKillboardInfoAsync(zKillHistory.Result))
                    };
                }
            }
            catch (Exception e)
            {
                _logger.LogError(e, "Could not fetch information from ZKillboard-Servers");
                throw;
            }

            return new EVEyePlayerDetails();
        }));

    #endregion

    #region helper methods

    private Task<EVEyeKillInformation>? GetEVEyeKillInformation(Task<EveDetailedKillInformation> latestKillboardActivity)
    {
        return latestKillboardActivity?.ContinueWith(latestKillboardActivityTask =>
        {
            if (latestKillboardActivityTask.Status == TaskStatus.RanToCompletion)
                return _eveDataRepository.GetNamesFrom(GetValidIDs(latestKillboardActivityTask.Result.Victim.ID, latestKillboardActivityTask.Result.Victim.ShipTypeID, latestKillboardActivityTask.Result.Attackers.First(x => x.FinalBlow).ID, latestKillboardActivityTask.Result.Attackers.First(x => x.FinalBlow).ShipTypeID, latestKillboardActivityTask!.Result!.Attackers.First(x => x.FinalBlow).WeaponTypeID, latestKillboardActivityTask.Result.SolarSystemID))
                    .ContinueWith(nameLookupTask =>
                {
                    if (nameLookupTask.Status == TaskStatus.RanToCompletion)
                        return new EVEyeKillInformation
                        {
                            Date = latestKillboardActivityTask.Result!.KillDate,
                            VictimName = nameLookupTask.Result.First(i => i.ID == latestKillboardActivityTask.Result.Victim.ID).Name,
                            VictimShip = nameLookupTask.Result.First(i => i.ID == latestKillboardActivityTask.Result.Victim.ShipTypeID).Name,
                            AttackerName = nameLookupTask.Result.First(i => i.ID == latestKillboardActivityTask.Result.Attackers.First(x => x.FinalBlow).ID).Name,
                            AttackerShip = nameLookupTask.Result.First(i => i.ID == latestKillboardActivityTask.Result.Attackers.First(x => x.FinalBlow).ShipTypeID).Name,
                            AttackerGuns = nameLookupTask.Result.First(i => i.ID == latestKillboardActivityTask.Result.Attackers.First(x => x.FinalBlow).WeaponTypeID).Name,
                            SolarSystem = nameLookupTask.Result.First(i => i.ID == latestKillboardActivityTask.Result.SolarSystemID).Name
                        };
                    _logger.LogError(nameLookupTask.Exception, "Could not fetch information from Servers");
                    throw new InvalidOperationException("Could not fetch information from Servers");
                });

            _logger.LogError(latestKillboardActivityTask.Exception, "Could not fetch information from Servers");
            throw new InvalidOperationException("Could not fetch information from Servers");

        }).Unwrap();
    }

    private Task<EveDetailedKillInformation> GetFirstDetailedKillboardInfoAsync(IEnumerable<ZKillboardEntry> killboardHistory, Func<ZKillboardEntry, bool>? killboardLookup = null)
    {
        var selectedKillboardEntry = killboardHistory.FirstOrDefault(killboardLookup ?? (_ => true));

        return selectedKillboardEntry is not null
            ? _eveDataRepository.GetDetailedKillInformation(selectedKillboardEntry.ID, selectedKillboardEntry.ZKillboardKill.Hash)
            : Task.FromResult(new EveDetailedKillInformation());
    }

    private static IEnumerable<int> GetValidIDs(params int[] IDs) => IDs.Where(i => i > 0);
    
    #endregion
}