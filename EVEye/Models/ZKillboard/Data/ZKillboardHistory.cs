using System.Text.Json.Serialization;

namespace EVEye.Models.ZKillboard.Data;

public class ZKillboardHistory
{
    [JsonPropertyName("killmail_id")] 
    public int ID { get; set; }

    [JsonPropertyName("zkb")] 
    public ZKillboardEntry ZKillboardEntry { get; set; }
}