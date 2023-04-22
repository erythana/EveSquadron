using System.Threading.Tasks;
using EveSquadron.Models;

namespace EveSquadron.DataRepositories.Interfaces;

public interface IGithubReleaseDataRepository
{
    public Task<GithubReleaseInformation> GetLatestReleaseInformationFrom(string endpoint);
}