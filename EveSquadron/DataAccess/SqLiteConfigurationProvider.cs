using System;
using System.Data;
using System.Data.SQLite;
using System.Linq;
using Dapper;
using EveSquadron.Models;
using Microsoft.Extensions.Configuration;

namespace EveSquadron.DataAccess;

public partial class SqLiteConfigurationProvider : ConfigurationProvider
{
    #region member fields
    
    private readonly string? _connectionString;
    
    #endregion

    #region constructor

    public SqLiteConfigurationProvider(string? connectionString)
    {
        _connectionString = connectionString;
    }
    
    #endregion
    
    #region overrides
    
    public override async void Load()
    {
        using IDbConnection dbConnection = new SQLiteConnection(_connectionString);
        var applicationSettings = await dbConnection.QueryAsync<ConfigurationValue>(SqLiteStatements.ApplicationSettings);
        Data = applicationSettings.ToDictionary(x => x.Name, x => x.Value, StringComparer.OrdinalIgnoreCase)!;
    }
    
    #endregion
}