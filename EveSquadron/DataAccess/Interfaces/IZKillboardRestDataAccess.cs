using System.Collections.Generic;
using System.Threading.Tasks;

namespace EveSquadron.DataAccess.Interfaces;

public interface IZKillboardRestDataAccess
{
    Task<T> GetCharacterStatisticsAsync<T>(string characterStatsEndpoint, int playerID);

    Task<IEnumerable<T>> GetKillboardHistoryFor<T>(string characterEndpoint, int playerID);
}