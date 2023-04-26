using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using EveSquadron.DataAccess.Interfaces;
using EveSquadron.DataRepositories.Interfaces;
using EveSquadron.Extensions;
using EveSquadron.Models;
using EveSquadron.Models.EVE.Data;
using EveSquadron.Models.EveSquadron;
using EveSquadron.Models.ZKillboard.Data;
using Microsoft.Extensions.Logging;

namespace EveSquadron.DataAccess;

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

    #region events
    
    public event EventHandler<(int? CorporationID, int? AllianceID)> ParsedNewID;

    #endregion

    #region interface implementation
    
    public async IAsyncEnumerable<EveSquadronPlayerInformation> GetAggregatedItemsFor(IEnumerable<string> players)
    {
        var eveNameIDMappings = await _eveDataRepository.GetIDsFrom(players);
        _logger.LogDebug($"Retrieved {eveNameIDMappings.Characters.Count} characters from GetIDsFrom.");
        
        foreach (var character in eveNameIDMappings.Characters)
        {
            var currentPlayer = new EveSquadronPlayerInformation
            {
                ID = character.ID,
                Character = character,
                CharacterImage = _eveDataRepository.GetPortraitFrom(character.ID, 32),
                PlayerDetails = GetLazyPlayerDetails(character)
            };

            //fire and forget - this one and the GetLazyPlayerDetails from above tasks may fail silently!
            SetCharacterInfo(currentPlayer).SafeFireAndForget(onException: e => _logger.LogError(e, "Could not fetch player Information"));
            yield return currentPlayer;
        }
    }

    private async Task SetCharacterInfo(EveSquadronPlayerInformation currentPlayer)
    {
        var charInfo = await _eveDataRepository.GetCharacterInformationFor(currentPlayer.ID);
        currentPlayer.SecurityStanding = charInfo.SecurityStatus;
        currentPlayer.Birthday = DateTime.Parse(charInfo.Birthday);

        var nameLookup = await _eveDataRepository.GetNamesFrom(GetValidIDs(charInfo.CorporationID, charInfo.AllianceID));
        var eveNameLookups = nameLookup.ToList();
        currentPlayer.Corporation = eveNameLookups.FirstOrDefault(c => c.ID == charInfo.CorporationID);
        currentPlayer.Alliance = eveNameLookups.FirstOrDefault(a => a.ID == charInfo.AllianceID);
        
        ParsedNewID.Invoke(this, (currentPlayer.Corporation?.ID, currentPlayer.Alliance?.ID));
    }

    private Lazy<Task<EveSquadronPlayerDetails>> GetLazyPlayerDetails(EveNameIDMapping character) =>
        new(() => Task.Run(async () =>
        {
            try
            {
                //Run these in sequence and also use a delay, zKillboard Rate Limit is harsh
                var zKillHistory = await _zKillboardDataRepository.GetKillboardHistoryFor(character.ID);
                await Task.Delay(AppConstants.ZKillboardAPILimits.ZKillboardRateLimitMs);
                var zKillStatistic = await _zKillboardDataRepository.GetStatisticsFrom(character.ID);
                return new EveSquadronPlayerDetails
                {
                    ID = character.ID,
                    DangerRatio = zKillStatistic.DangerRatio,
                    GangRatio = zKillStatistic.GangRatio,
                    ShipsDestroyed = zKillStatistic.ShipsDestroyed,
                    ShipsLost = zKillStatistic.ShipsLost,
                    SoloKills = zKillStatistic.SoloKills,
                    SoloLosses = zKillStatistic.SoloLosses,
                    LatestKillboardActivity = GetEveSquadronKillInformation(GetFirstDetailedKillboardInfoAsync(zKillHistory))
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

    private async Task<EveSquadronKillInformation>? GetEveSquadronKillInformation(Task<EveDetailedKillInformation> latestKillboardActivity)
    {
        var latestKillboard = await latestKillboardActivity;

        var nameLookup = await _eveDataRepository.GetNamesFrom(GetValidIDs(latestKillboard.Victim.ID, latestKillboard.Victim.ShipTypeID,
            latestKillboard.Attackers.First(x => x.FinalBlow).ID, latestKillboard.Attackers.First(x => x.FinalBlow).ShipTypeID,
            latestKillboard.Attackers.First(x => x.FinalBlow).WeaponTypeID, latestKillboard.SolarSystemID));

        var eveNameLookups = nameLookup.ToList();
        return new EveSquadronKillInformation()
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