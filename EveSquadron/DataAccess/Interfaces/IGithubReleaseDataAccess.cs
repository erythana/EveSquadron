using System.Threading.Tasks;

namespace EveSquadron.DataAccess.Interfaces;

public interface IGithubReleaseDataAccess
{
    public Task<T> GetLatestReleaseInformation<T>(string endpoint);
}