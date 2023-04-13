using System.Collections.Generic;
using System.Threading.Tasks;
using EVEye.Models.ZKillboard.Data;

namespace EVEye.Models.ZKillboard.Interfaces;

public interface IZKillboardDataRepository
{
    public Task<ZKillboardCharacterStatistic> GetStatisticsFrom(int playerID, int delay = 0);

    public Task<IEnumerable<ZKillboardEntry>> GetKillboardHistoryFor(int playerID, int delay = 0);
}