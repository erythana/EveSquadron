namespace EveSquadron.Models.Interfaces;

public interface IEveESIEndpointsLoader
{
    public string UniverseEndpoint { get; }
    public string CharacterEndpoint { get; }
    public string PortraitEndpoint { get; }
    string KillmailEndpoint { get; }
}