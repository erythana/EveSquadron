using System;
using System.Text.Json.Serialization;

namespace EveSquadron.Models.EVE.Data;

public class EveCharacter
{
    public string Name { get; set; }
    public string Birthday { get; set; }
    [JsonPropertyName("Corporation_ID")]
    public int CorporationID { get; set; }
    [JsonPropertyName("Alliance_ID")]
    public int AllianceID { get; set; }
    [JsonPropertyName("Security_Status")]
    public float SecurityStatus { get; set; }
}