using System.Collections.Generic;
using System.Threading.Tasks;
using EveSquadron.Models.EveSquadron.Interfaces;

namespace EveSquadron.DataAccess.Interfaces;

public interface IWhitelistManagementSqLiteDataAccess
{
    public Task CreateAndConnectToDBFile(string databaseDirectory);
    public Task<IEnumerable<IWhitelistEntry>> LoadWhitelistedCharacters<T>();
    public Task SaveWhitelistedCharacters(IEnumerable<IWhitelistEntry> whitelistedEntities);
}