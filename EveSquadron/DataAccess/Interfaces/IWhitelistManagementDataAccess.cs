using System.Collections.Generic;
using System.Threading.Tasks;
using EveSquadron.Models.Interfaces;

namespace EveSquadron.DataAccess.Interfaces;

public interface IWhitelistManagementSqLiteDataAccess
{
    public Task<IEnumerable<IWhitelistEntry>> LoadWhitelistedCharacters<T>();
    public Task SaveWhitelistedCharacters(IEnumerable<IWhitelistEntry> whitelistedEntities);
}