namespace EveSquadron.Models.Interfaces;

public interface IZKillboardEndpointsLoader
{
    public string CharacterEndpoint { get; }
    public string CharacterStatsEndpoint { get; }
}