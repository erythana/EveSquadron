namespace EveSquadron.Models.Interfaces;

public interface IReleaseSettingsLoader
{
    string ReleasePath { get; }
    string ReleaseVersionAPIEndpoint { get; }
}