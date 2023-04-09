using System.Threading.Tasks;
using EVEye.Models.ZKillboard.Data;

namespace EVEye.Models.ZKillboard.Interfaces
{
    public interface IZKillboardDataRepository
    {
        public Task<ZKillboardCharacterStatistic?> GetStatisticsFrom(int playerID);
    }
}