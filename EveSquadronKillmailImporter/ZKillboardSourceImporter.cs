using System.Net.Http.Headers;
using System.Text.Json;
using EveSquadronKillmailImporter.Interfaces;
using EveSquadronKillmailImporter.Models;
using Microsoft.Extensions.Configuration;

namespace EveSquadronKillmailImporter;

internal class ZKillboardSourceImporter : IProcessKillmails
{
    #region member fields
    
    private readonly IEveSquadronDataRepository _eveSquadronDataRepository;
    private readonly HttpClient _httpClient;
    private readonly string _zkillboardRedisEndpoint;
    private readonly bool _keepListening;
    private readonly int _bulkInsertSize;
    
    #endregion

    #region constructor

    public ZKillboardSourceImporter(IConfiguration configuration, IEveSquadronDataRepository eveSquadronDataRepository)
    {
        _eveSquadronDataRepository = eveSquadronDataRepository;
        _keepListening = configuration.GetValue<bool>("ZkillboardRedisSettings:KeepListening");
        _bulkInsertSize = configuration.GetValue("ZkillboardRedisSettings:BulkInsertSize", 20);
        _zkillboardRedisEndpoint = configuration.GetSection("ZkillboardRedisSettings:Endpoint").Value!;

        if (string.IsNullOrWhiteSpace(_zkillboardRedisEndpoint))
            throw new InvalidOperationException("Did not find 'ZkillboardRedisSettings:Endpoint' setting.");

        _httpClient = new HttpClient();
        _httpClient.DefaultRequestHeaders.TryAddWithoutValidation("User-Agent", "EveSquadronKillmailImporter - erythanadevsup@gmail.com");
    }
    
    #endregion

    #region interface implementation

    public async Task ProcessKillmails()
    {
        var saveableKillmails = new List<Killmail>();
        try
        {
            while (true)
            {
                var redisResponse = await _httpClient.GetStringAsync(_zkillboardRedisEndpoint);
                var redisPackage = JsonSerializer.Deserialize<ZKillboardRedisPackage>(redisResponse, new JsonSerializerOptions { PropertyNameCaseInsensitive = true });

                if (redisPackage?.Package is not null)
                    saveableKillmails.Add(GetKillmailFromPackage(redisPackage));

                if (redisPackage?.Package is null || saveableKillmails.Count >= _bulkInsertSize)
                {
                    var alreadySavedKillmails = await _eveSquadronDataRepository.GetAlreadySavedKillmails(saveableKillmails);
                    var missingKillmails = saveableKillmails.Where(x => !alreadySavedKillmails.Contains(x.ID));
                    
                    await _eveSquadronDataRepository.SaveKillmails(missingKillmails);
                    saveableKillmails.Clear();
                }
                
                if (!_keepListening && redisPackage?.Package is null) //Stop execution when there are no new killmails and we don't want to run indefinitely.
                    break;
            }
            
        }
        catch (Exception e)
        {
            Console.WriteLine("Exception in ZKillboardSourceImporter:");
            Console.WriteLine(e);
            throw;
        }
    }
    
    #endregion

    #region helper methods
    
    private Killmail GetKillmailFromPackage(ZKillboardRedisPackage redisPackage) => new()
    {
        ID = redisPackage.Package.ID,
        KillmailDate = redisPackage.Package.Killmail.killmail_time,
        KillmailHash = redisPackage.Package.ZKillboardKill.Hash,
        SolarSystemID = redisPackage.Package.Killmail.solar_system_id,
        WinCharacterID = redisPackage.Package.Killmail.attackers.FirstOrDefault(x => x.final_blow)?.character_id,
        WinShipTypeID = redisPackage.Package.Killmail.attackers.FirstOrDefault(x => x.final_blow)?.ship_type_id,
        WinWeaponTypeID = redisPackage.Package.Killmail.attackers.FirstOrDefault(x => x.final_blow)?.weapon_type_id,
        WinAttackerCount = redisPackage.Package.Killmail.attackers.Count,
        LossCharacterID = redisPackage.Package.Killmail.victim.character_id,
        LossShipTypeID = redisPackage.Package.Killmail.victim.ship_type_id,
    };
    
    #endregion

}