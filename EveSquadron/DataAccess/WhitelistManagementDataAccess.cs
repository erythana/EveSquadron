using System.Collections.Generic;
using System.Data;
using System.Data.SQLite;
using System.Threading.Tasks;
using Dapper;
using EveSquadron.DataAccess.Interfaces;
using EveSquadron.Models;
using EveSquadron.Models.Interfaces;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;

namespace EveSquadron.DataAccess;

public class WhitelistManagementSqLiteDataAccess : IWhitelistManagementSqLiteDataAccess
{
    #region member fields
    
    private readonly ILogger<IWhitelistManagementSqLiteDataAccess> _logger;
    private readonly string _connectionString;

    #endregion

    #region constructor

    public WhitelistManagementSqLiteDataAccess(IConfiguration configuration, ILogger<IWhitelistManagementSqLiteDataAccess> logger, string connectionID = "Default")
    {
        _logger = logger;
        _connectionString = ConnectionStringHelper.GetConnectionString(configuration);
    }
    #endregion

    #region interface implementation
    
    public async Task<IEnumerable<IWhitelistEntry>> LoadWhitelistedCharacters<T>()
    {
        using IDbConnection sqLiteConnection = new SQLiteConnection(_connectionString);
        var returnValues = await sqLiteConnection.QueryAsync<WhitelistEntry>("select * from WhitelistEntries");
        return returnValues;
    }

    public async Task SaveWhitelistedCharacters(IEnumerable<IWhitelistEntry> whitelistedEntities)
    {
        using IDbConnection sqLiteConnection = new SQLiteConnection(_connectionString);
        await sqLiteConnection.ExecuteAsync("delete from WhitelistEntries"); //yolo - shouldn't be that many lines anyway
        await sqLiteConnection.ExecuteAsync("insert into WhitelistEntries (Type, Name) values(@Type, @Name);", whitelistedEntities);
    }

    #endregion
}