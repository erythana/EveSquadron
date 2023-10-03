using System.ComponentModel.DataAnnotations;

namespace EveSquadron.Models.Options;

public class ZkillboardEndpointOptions
{
    public const string Section = "Endpoints:Zkillboard";
    
    [Required]
    public string CharacterEndpoint { get; set; }
    
    [Required]
    public string CharacterStatsEndpoint { get; set; }
}