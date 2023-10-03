using System.ComponentModel.DataAnnotations;

namespace EveSquadron.Models.Options;

public class EveEndpointOptions
{
    public const string Section = "Endpoints:EveESI";
    
    [Required]
    public string UniverseEndpoint { get; set; }
    
    [Required]
    public string CharacterEndpoint { get; set; }
    
    [Required]
    public string PortraitEndpoint { get; set; }
    
    [Required]
    public string KillmailEndpoint { get; set; }
}