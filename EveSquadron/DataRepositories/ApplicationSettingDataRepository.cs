using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using EveSquadron.DataAccess.Interfaces;
using EveSquadron.DataRepositories.Interfaces;
using EveSquadron.Models;
using Microsoft.Extensions.Logging;

namespace EveSquadron.DataRepositories;

public class ApplicationSettingDataRepository : IApplicationSettingDataRepository
{
    #region member fields

    private readonly IApplicationSettingsDataAccess _applicationSettingsDataAccess;
    private readonly ILogger<IApplicationSettingDataRepository> _logger;

    #endregion

    #region constructor

    public ApplicationSettingDataRepository(IApplicationSettingsDataAccess applicationSettingsDataAccess, ILogger<IApplicationSettingDataRepository> logger)
    {
        _applicationSettingsDataAccess = applicationSettingsDataAccess;
        _logger = logger;
    }
    
    #endregion

    #region interface implemenetation

    public async Task SaveApplicationSettings(IEnumerable<ConfigurationValue> configurationValues)
    {
        if (configurationValues.Any())
            await _applicationSettingsDataAccess.SaveApplicationSettings(configurationValues);
    }


    #endregion

}