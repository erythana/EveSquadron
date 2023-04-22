using System.Collections.Generic;
using System.Threading.Tasks;
using Avalonia.Media.Imaging;
using EveSquadron.Models.EVE.Data;

namespace EveSquadron.DataRepositories.Interfaces;

public interface IEveDataRepository
{
    public Task<EveUniverseIDMapping> GetIDsFrom(IEnumerable<string> names);
    public Task<IEnumerable<EveNameLookup>> GetNamesFrom(IEnumerable<int> id);
    public Task<Bitmap?> GetPortraitFrom(int characterID, int width);
    public Task<EveDetailedKillInformation> GetDetailedKillInformation(int killmailID, string killmailHash);
    public Task<EveCharacter> GetCharacterInformationFor(int playerID);
}