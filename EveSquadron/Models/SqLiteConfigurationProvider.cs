using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SQLite;
using System.Linq;
using Dapper;
using Microsoft.Extensions.Configuration;

namespace EveSquadron.Models;

public partial class SqLiteConfigurationProvider : ConfigurationProvider
{
    private readonly string? _connectionString;

    public SqLiteConfigurationProvider(string? connectionString)
    {
        _connectionString = connectionString;
    }
    
    public override async void Load()
    {
        using IDbConnection dbConnection = new SQLiteConnection(_connectionString);
        var applicationSettings = await dbConnection.QueryAsync<ConfigurationValue>(SqLiteStatements.ApplicationSettings);
        Data = applicationSettings.ToDictionary(x => x.Name, x => x.Value, StringComparer.OrdinalIgnoreCase)!;
    }

    public override void Set(string key, string? value)
    {
        base.Set(key, value);
    }
}