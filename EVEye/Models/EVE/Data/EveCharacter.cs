using System;
using System.Text.Json.Serialization;

namespace EVEye.Models.EVE.Data;

public class EveCharacter
{
    public string Name { get; set; }
    public string Birthday { get; set; }
    [JsonPropertyName("Security_Status")]
    public float SecurityStatus { get; set; }
}