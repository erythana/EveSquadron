using System.Collections.Generic;
using System.Data;
using System.Data.SQLite;
using System.Threading.Tasks;
using Dapper;
using EveSquadron.DataAccess.Base;
using EveSquadron.DataAccess.Interfaces;
using EveSquadron.Models;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;

namespace EveSquadron.DataAccess;

public partial class ApplicationSettingsDataAccess : SqLiteDataAccessBase, IApplicationSettingsDataAccess
{
    #region constructor

    public ApplicationSettingsDataAccess(IConfiguration configuration, ILogger<SqLiteDataAccessBase> logger) : base(configuration, logger)
    {
    }

    #endregion

    #region interface implementation

    public async Task SaveApplicationSettings(IEnumerable<ConfigurationValue> configurationValues)
    {
        using IDbConnection sqLiteConnection = new SQLiteConnection(ConnectionString);
        await sqLiteConnection.ExecuteAsync(SqLiteStatements.InsertReplaceApplicationSettings, configurationValues);
    }

    #endregion
}