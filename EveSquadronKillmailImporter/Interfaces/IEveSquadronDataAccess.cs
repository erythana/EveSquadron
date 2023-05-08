using System.Data;
using EveSquadronKillmailImporter.Models;

namespace EveSquadronKillmailImporter.Interfaces;

internal interface IEveSquadronDataAccess
{
    public Task<IEnumerable<int>>  GetAlreadyCreatedKillmailIDs(IEnumerable<int> checkableIDs);
    
    public Task SaveKillmails(DataTable killmailDataTable);
}