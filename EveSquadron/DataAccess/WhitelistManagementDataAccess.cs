using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SQLite;
using System.IO;
using System.Threading.Tasks;
using Dapper;
using EveSquadron.DataAccess.Interfaces;
using EveSquadron.Extensions;
using EveSquadron.Models.EveSquadron;
using EveSquadron.Models.EveSquadron.Interfaces;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;

namespace EveSquadron.DataAccess;

public class WhitelistManagementSqLiteDataAccess : IWhitelistManagementSqLiteDataAccess
{
    #region member fields

    private readonly IConfiguration _configuration;
    private readonly ILogger<IWhitelistManagementSqLiteDataAccess> _logger;
    private string _connectionString;

    #endregion

    #region constructor

    public WhitelistManagementSqLiteDataAccess(IConfiguration configuration, ILogger<IWhitelistManagementSqLiteDataAccess> logger, string connectionID = "Default")
    {
        _configuration = configuration;
        _logger = logger;
        _connectionString = _configuration.GetConnectionString(connectionID) ?? string.Empty;
    }

    private bool ValidateAndSanitizeConnectionString(string databaseDirectory)
    {
        if (string.IsNullOrWhiteSpace(_connectionString))
        {
            _logger.LogCritical("Invalid connection string for WhitelistManagement. Is the '{{0}}' placeholder listed?");
            return false;
        }

        if (_connectionString.Contains("{0}"))
        {
            if (!Directory.Exists(databaseDirectory) || !DirectoryHelper.HasWriteAccess(new DirectoryInfo(databaseDirectory)))
            {
                _logger.LogCritical($"Database directory does not exist or no write access to directory: '{databaseDirectory}'");
                return false;
            }

            _connectionString = string.Format(_connectionString, databaseDirectory);
        }

        return true;
    }

    #endregion

    #region interface implementation

    public Task CreateAndConnectToDBFile(string databaseDirectory = null)
    {
        if (!ValidateAndSanitizeConnectionString(databaseDirectory))
            throw new ArgumentException($"The connection string '{_configuration}' does not point to a valid, writeable sqlite database.");

        using IDbConnection sqLiteConnection = new SQLiteConnection(_connectionString);
        TryInitializeDatabase(sqLiteConnection);

        return Task.CompletedTask;
    }

    private void TryInitializeDatabase(IDbConnection dbConnection)
    {
        dbConnection.Execute(@"
        CREATE TABLE IF NOT EXISTS [WhitelistEntries] (
            [Type] INTEGER NOT NULL,
            [Name] text NOT NULL
        )");
    }

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