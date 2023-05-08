using EveSquadronKillmailImporter.Models;

namespace EveSquadronKillmailImporter.Interfaces;

internal interface IEveSquadronDataRepository
{
    public Task<IEnumerable<int>> GetAlreadySavedKillmails(IEnumerable<Killmail> killmails);

    public Task SaveKillmails(IEnumerable<Killmail> killmails);
}