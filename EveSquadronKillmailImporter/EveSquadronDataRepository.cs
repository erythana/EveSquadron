using System.Data;
using EveSquadronKillmailImporter.Interfaces;
using EveSquadronKillmailImporter.Models;

namespace EveSquadronKillmailImporter;

internal class EveSquadronDataRepository : IEveSquadronDataRepository
{
    private readonly IEveSquadronDataAccess _eveSquadronDataAccess;

    public EveSquadronDataRepository(IEveSquadronDataAccess eveSquadronDataAccess)
    {
        _eveSquadronDataAccess = eveSquadronDataAccess;
    }

    public Task<IEnumerable<int>> GetAlreadySavedKillmails(IEnumerable<Killmail> killmails) => _eveSquadronDataAccess.GetAlreadyCreatedKillmailIDs(killmails.Select(x => x.ID));

    public Task SaveKillmails(IEnumerable<Killmail> killmails)
    {
        var dataTable = killmails.ToDataTable();
        return _eveSquadronDataAccess.SaveKillmails(dataTable);
    }
}