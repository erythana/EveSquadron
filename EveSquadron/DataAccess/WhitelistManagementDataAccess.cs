using System.Collections.Generic;
using System.Data;
using System.Data.SQLite;
using System.Threading.Tasks;
using Dapper;
using EveSquadron.DataAccess.Base;
using EveSquadron.DataAccess.Interfaces;
using EveSquadron.Models;
using EveSquadron.Models.Interfaces;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;

namespace EveSquadron.DataAccess;

public class WhitelistManagementSqLiteDataAccess : SqLiteDataAccessBase, IWhitelistManagementSqLiteDataAccess
{
    #region member fields
    
    private readonly ILogger<IWhitelistManagementSqLiteDataAccess> _logger;

    #endregion

    #region constructor

    public WhitelistManagementSqLiteDataAccess(IConfiguration configuration, ILogger<WhitelistManagementSqLiteDataAccess> logger) : base(configuration, logger)
    {
        _logger = logger;
        
    }
    #endregion

    #region interface implementation
    
    public async Task<IEnumerable<IWhitelistEntry>> LoadWhitelistedCharacters<T>()
    {
        using IDbConnection sqLiteConnection = new SQLiteConnection(ConnectionString);
        var returnValues = await sqLiteConnection.QueryAsync<WhitelistEntry>("select * from WhitelistEntries");
        return returnValues;
    }

    public async Task SaveWhitelistedCharacters(IEnumerable<IWhitelistEntry> whitelistedEntities)
    {
        using IDbConnection sqLiteConnection = new SQLiteConnection(ConnectionString);
        await sqLiteConnection.ExecuteAsync("delete from WhitelistEntries"); //yolo - shouldn't be that many lines anyway
        await sqLiteConnection.ExecuteAsync("insert into WhitelistEntries (Type, Name) values(@Type, @Name);", whitelistedEntities);
    }

    #endregion
}