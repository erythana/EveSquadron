using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;
using Avalonia.Media.Imaging;
using EveSquadron.DataAccess.Interfaces;
using EveSquadron.DataRepositories.Interfaces;
using EveSquadron.Models.EVE.Data;
using EveSquadron.Models.Interfaces;
using EveSquadron.Models.Options;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

namespace EveSquadron.DataRepositories;

public class EveDataRepository : IEveDataRepository
{
    #region member fields

    private readonly IEveRestDataAccess _eveRestDataAccess;
    private readonly ILogger<EveDataRepository> _logger;

    private readonly string _universeEndpoint;
    private readonly string _killmailEndpoint;
    private readonly string _portraitEndpoint;
    private readonly string _characterEndpoint;

    #endregion
    
    #region constructor

    public EveDataRepository(IEveRestDataAccess eveRestDataAccess, IOptions<EveEndpointOptions> endpointOptions, ILogger<EveDataRepository> logger)
    {
        _eveRestDataAccess = eveRestDataAccess;
        _logger = logger;

        var option = endpointOptions.Value;
        _universeEndpoint = option.UniverseEndpoint;
        _characterEndpoint = option.CharacterEndpoint;
        _killmailEndpoint = option.KillmailEndpoint;
        _portraitEndpoint = option.PortraitEndpoint;
    }

    #endregion

    #region interface methods

    public Task<EveUniverseIDMapping> GetIDsFrom(IEnumerable<string> names) => _eveRestDataAccess.GetIDsFromNames<EveUniverseIDMapping>(_universeEndpoint + "ids/", names);

    public Task<IEnumerable<EveNameLookup>> GetNamesFrom(IEnumerable<int> ids) => _eveRestDataAccess.GetNamesFromIDs<EveNameLookup>(_universeEndpoint + "names/", ids);

    public async Task<Bitmap?> GetPortraitFrom(int characterID, int width)
    {
        var imageContent = await _eveRestDataAccess.GetPortraitByteArrayAsync(_portraitEndpoint + characterID + $"/portrait?size={width}");
        var stream = new MemoryStream(imageContent);
        return Bitmap.DecodeToWidth(stream, width);
    }

    public Task<EveDetailedKillInformation> GetDetailedKillInformation(int killmailID, string killmailHash) => _eveRestDataAccess.GetDetailedKillInformationFor<EveDetailedKillInformation>(_killmailEndpoint, killmailHash, killmailID);

    public Task<EveCharacter> GetCharacterInformationFor(int playerID) => _eveRestDataAccess.GetCharacterInformationFor<EveCharacter>(_characterEndpoint, playerID);

    #endregion
}