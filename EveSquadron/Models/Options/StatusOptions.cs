namespace EveSquadron.Models.Options;

public class StatusOptions
{
    public const string Section = "ApplicationState";
    
    public string WhitelistActive { get; set;}
    
    public string AlwaysOnTop { get; set;}
}