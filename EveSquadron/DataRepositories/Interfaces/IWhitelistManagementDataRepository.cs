using System.Collections.Generic;
using System.Threading.Tasks;
using EveSquadron.Models.Interfaces;

namespace EveSquadron.DataRepositories.Interfaces;

public interface IWhitelistManagementDataRepository
{
    public Task<IEnumerable<IWhitelistEntry>> LoadWhitelistedEntities();

    public Task SaveWhitelistedEntities(IEnumerable<IWhitelistEntry> whitelistEntries);
}