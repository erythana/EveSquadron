using System.Collections.Generic;
using System.Threading.Tasks;

namespace EveSquadron.DataAccess.Interfaces;

public interface IEveRestDataAccess
{
    public Task<T> GetIDsFromNames<T>(string idEndpoint, IEnumerable<string> names);
    public Task<byte[]> GetPortraitByteArrayAsync(string portraitEndpoint);
    Task<IEnumerable<T>> GetNamesFromIDs<T>(string namesEndpoint, IEnumerable<int> ids);
    Task<T> GetDetailedKillInformationFor<T>(string killmailsEndpoint, string killmailHash, int killmailID);
    public Task<T> GetCharacterInformationFor<T>(string characterEndpoint, int playerID);
}