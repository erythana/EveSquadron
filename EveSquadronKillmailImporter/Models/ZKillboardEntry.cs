using System.Text.Json.Serialization;

namespace EveSquadronKillmailImporter.Models;

public class ZKillboardRedisEntry
{
    [JsonPropertyName("killID")] 
    public int ID { get; set; }
    
    [JsonPropertyName("killmail")] 
    public EveKillmail Killmail { get; set; }

    [JsonPropertyName("zkb")] 
    public ZKillboardKill ZKillboardKill { get; set; }
}