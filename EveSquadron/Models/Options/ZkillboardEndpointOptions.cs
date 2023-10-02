using System.ComponentModel.DataAnnotations;

namespace EveSquadron.Models.Options;

public class ZkillboardEndpointOptions
{
    [Required]
    public string CharacterEndpoint { get; set; }
    
    [Required]
    public string CharacterStatsEndpoint { get; set; }
}