using System.ComponentModel.DataAnnotations;

namespace EveSquadron.Models.Options;

public class ReleaseEndpointOptions
{
    public const string Section = "Endpoints:Release";
    
    [Required]
    public string ReleasePath { get; set; }
    
    [Required]
    public string ReleaseVersionAPIEndpoint { get; set; }
}