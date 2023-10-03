using System.Collections.Generic;
using System.Threading.Tasks;
using EveSquadron.Models;

namespace EveSquadron.DataAccess.Interfaces;

public interface IApplicationSettingsDataAccess
{
    Task SaveApplicationSettings(IEnumerable<ConfigurationValue> configurationValues);
}