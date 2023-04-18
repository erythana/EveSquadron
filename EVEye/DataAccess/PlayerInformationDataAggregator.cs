using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using EVEye.DataAccess.Interfaces;
using EVEye.Extensions;
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
            var currentPlayer = new EVEyePlayerInformation
            {
                ID = character.ID,
                CharacterName = character.Name,
                CharacterImage = _eveDataRepository.GetPortraitFrom(character.ID, 32),
                PlayerDetails = GetLazyPlayerDetails(character)
            };

            //fire and forget - this one and the GetLazyPlayerDetails from above tasks may fail silently!
            SetCharacterInfo(currentPlayer).SafeFireAndForget(onException: e => _logger.LogError(e, "Could not fetch player Information"));
            yield return currentPlayer;
        }
    }

    private async Task SetCharacterInfo(EVEyePlayerInformation currentPlayer)
    {
        var charInfo = await _eveDataRepository.GetCharacterInformationFor(currentPlayer.ID);
        currentPlayer.SecurityStanding = charInfo.SecurityStatus;
        currentPlayer.Birthday = DateTime.Parse(charInfo.Birthday);

        var nameLookup = await _eveDataRepository.GetNamesFrom(GetValidIDs(charInfo.CorporationID, charInfo.AllianceID));
        var eveNameLookups = nameLookup.ToList();
        currentPlayer.CorporationName = eveNameLookups.FirstOrDefault(c => c.ID == charInfo.CorporationID)?.Name;
        currentPlayer.AllianceName = eveNameLookups.FirstOrDefault(a => a.ID == charInfo.AllianceID)?.Name;
    }

    private Lazy<Task<EVEyePlayerDetails>> GetLazyPlayerDetails(EveNameIDMapping character) =>
        new(() => Task.Run(async () =>
        {
            try
            {
                //Run these in sequence and also use a delay, zKillboard Rate Limit is harsh
                var zKillHistory = await _zKillboardDataRepository.GetKillboardHistoryFor(character.ID);
                await Task.Delay(ApplicationConstants.ZKillboardAPILimits.ZKillboardRateLimitMs);
                var zKillStatistic = await _zKillboardDataRepository.GetStatisticsFrom(character.ID);
                return new EVEyePlayerDetails
                {
                    ID = character.ID,
                    DangerRatio = zKillStatistic.DangerRatio,
                    GangRatio = zKillStatistic.GangRatio,
                    ShipsDestroyed = zKillStatistic.ShipsDestroyed,
                    ShipsLost = zKillStatistic.ShipsLost,
                    SoloKills = zKillStatistic.SoloKills,
                    SoloLosses = zKillStatistic.SoloLosses,
                    LatestKillboardActivity = GetEVEyeKillInformation(GetFirstDetailedKillboardInfoAsync(zKillHistory))
                };
            }
            catch (Exception e)
            {
                _logger.LogError(e, "Could not fetch information from ZKillboard-Servers");
                throw;
            }
        }));

    #endregion

    #region helper methods

    private async Task<EVEyeKillInformation>? GetEVEyeKillInformation(Task<EveDetailedKillInformation> latestKillboardActivity)
    {
        var latestKillboard = await latestKillboardActivity;

        var nameLookup = await _eveDataRepository.GetNamesFrom(GetValidIDs(latestKillboard.Victim.ID, latestKillboard.Victim.ShipTypeID,
            latestKillboard.Attackers.First(x => x.FinalBlow).ID, latestKillboard.Attackers.First(x => x.FinalBlow).ShipTypeID,
            latestKillboard.Attackers.First(x => x.FinalBlow).WeaponTypeID, latestKillboard.SolarSystemID));

        var eveNameLookups = nameLookup.ToList();
        return new EVEyeKillInformation()
        {
            Date = latestKillboard.KillDate,
            VictimName = eveNameLookups.First(i => i.ID == latestKillboard.Victim.ID).Name,
            VictimShip = eveNameLookups.First(i => i.ID == latestKillboard.Victim.ShipTypeID).Name,
            AttackerName = eveNameLookups.First(i => i.ID == latestKillboard.Attackers.First(x => x.FinalBlow).ID).Name,
            AttackerShip = eveNameLookups.First(i => i.ID == latestKillboard.Attackers.First(x => x.FinalBlow).ShipTypeID).Name,
            AttackerGuns = eveNameLookups.First(i => i.ID == latestKillboard.Attackers.First(x => x.FinalBlow).WeaponTypeID).Name,
            SolarSystem = eveNameLookups.First(i => i.ID == latestKillboard.SolarSystemID).Name
        };
    }

    private Task<EveDetailedKillInformation> GetFirstDetailedKillboardInfoAsync(IEnumerable<ZKillboardEntry> killboardHistory, Func<ZKillboardEntry, bool>? killboardLookup = null)
    {
        var selectedKillboardEntry = killboardHistory.FirstOrDefault(killboardLookup ?? (_ => true));

        return selectedKillboardEntry is not null
            ? _eveDataRepository.GetDetailedKillInformation(selectedKillboardEntry.ID, selectedKillboardEntry.ZKillboardKill.Hash)
            : Task.FromResult(new EveDetailedKillInformation());
    }

    private static IEnumerable<int> GetValidIDs(params int[] ids) => ids.Where(i => i > 0).Distinct();

    #endregion
}