using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;
using EveSquadron.DataAccess.Interfaces;
using EveSquadron.DataRepositories.Interfaces;
using EveSquadron.Models.EveSquadron;
using EveSquadron.Models.Interfaces;
using EveSquadron.Models.ZKillboard.Data;
using Microsoft.Extensions.Logging;

namespace EveSquadron.DataRepositories;

public class WhitelistManagementDataRepository : IWhitelistManagementDataRepository
{
    #region member fields

    private readonly IWhitelistManagementSqLiteDataAccess _whitelistManagementSqLiteDataAccess;
    private readonly ILogger<IWhitelistManagementDataRepository> _logger;

    #endregion

    #region constructor

    public WhitelistManagementDataRepository(IWhitelistManagementSqLiteDataAccess whitelistManagementSqLiteDataAccess, ILogger<IWhitelistManagementDataRepository> logger)
    {
        _whitelistManagementSqLiteDataAccess = whitelistManagementSqLiteDataAccess;
        _logger = logger;
    }
    
    #endregion

    #region interface implemenetation

    public Task<IEnumerable<IWhitelistEntry>> LoadWhitelistedEntities()
    {
        return _whitelistManagementSqLiteDataAccess.LoadWhitelistedCharacters<IWhitelistEntry>();
    }
    
    public Task SaveWhitelistedEntities(IEnumerable<IWhitelistEntry> whitelistEntries)
    {
        return _whitelistManagementSqLiteDataAccess.SaveWhitelistedCharacters(whitelistEntries);
    }

    #endregion
    
}