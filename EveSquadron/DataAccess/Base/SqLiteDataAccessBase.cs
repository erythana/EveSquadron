using EveSquadron.Models;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;

namespace EveSquadron.DataAccess.Base;

public abstract class SqLiteDataAccessBase
{
    #region member fields

    protected readonly string ConnectionString;

    #endregion

    #region constructor
    
    protected SqLiteDataAccessBase(IConfiguration configuration, ILogger<SqLiteDataAccessBase> logger)
    {
        ConnectionString = ConnectionStringHelper.GetConnectionString(configuration);
    }
    
    #endregion
}