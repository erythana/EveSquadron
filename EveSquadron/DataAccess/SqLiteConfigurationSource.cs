using System.Data;
using System.Data.SQLite;
using Dapper;
using EveSquadron.Models;
using Microsoft.Extensions.Configuration;

namespace EveSquadron.DataAccess;

public partial class SqLiteConfigurationSource : IConfigurationSource
{
    #region member fields

    private readonly string? _connectionString;

    #endregion

    #region constructor
    
    public SqLiteConfigurationSource(IConfiguration configuration)
    {
        _connectionString = ConnectionStringHelper.GetConnectionString(configuration);
        
        using IDbConnection sqLiteConnection = new SQLiteConnection(_connectionString);
        InitializeDatabase(sqLiteConnection);
    }
    
    #endregion
    
    #region interface implementation
    
    public IConfigurationProvider Build(IConfigurationBuilder builder) => new SqLiteConfigurationProvider(_connectionString);
    
    #endregion

    #region helper methods

    private void InitializeDatabase(IDbConnection dbConnection)
    {
        dbConnection.Execute(SqLiteStatements.CreateApplicationSettingsTable);
        dbConnection.Execute(SqLiteStatements.CreateWhitelistEntriesTable);
    }

    #endregion
}