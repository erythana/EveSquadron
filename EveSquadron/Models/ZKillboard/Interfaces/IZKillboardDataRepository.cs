using System.Collections.Generic;
using System.Threading.Tasks;
using EveSquadron.Models.ZKillboard.Data;

namespace EveSquadron.Models.ZKillboard.Interfaces;

public interface IZKillboardDataRepository
{
    public Task<ZKillboardCharacterStatistic> GetStatisticsFrom(int playerID);

    public Task<IEnumerable<ZKillboardEntry>> GetKillboardHistoryFor(int playerID);
}