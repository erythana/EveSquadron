using System.Collections.Generic;
using System.Threading.Tasks;
using EveSquadron.Models;

namespace EveSquadron.DataRepositories.Interfaces;

public interface IApplicationSettingDataRepository
{
    public Task SaveApplicationSettings(IEnumerable<ConfigurationValue> configurationValues);
}