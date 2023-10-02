using System.ComponentModel.DataAnnotations;

namespace EveSquadron.Models.Options;

public class ReleaseEndpointOptions
{
    [Required]
    public string ReleasePath { get; set; }
    
    [Required]
    public string ReleaseVersionAPIEndpoint { get; set; }
}