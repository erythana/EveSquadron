using System.Collections.Generic;
using System.Threading.Tasks;
using EVEye.Models.ZKillboard.Data;

namespace EVEye.Models.ZKillboard.Interfaces;

public interface IZKillboardDataRepository
{
    public Task<ZKillboardCharacterStatistic?> GetStatisticsFrom(int playerID);

    public Task<IEnumerable<ZKillboardHistory>?> GetKillboardHistoryFor(int playerID);
}