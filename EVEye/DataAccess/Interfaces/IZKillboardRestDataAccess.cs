using System.Threading.Tasks;

namespace EVEye.DataAccess.Interfaces
{
    public interface IZKillboardRestDataAccess
    {
        Task<T?> GetCharacterStatisticsAsync<T>(string characterStatsEndpoint, int playerID);
    }
}