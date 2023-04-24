using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text.Json;
using System.Threading.Tasks;
using EveSquadron.DataAccess.Base;
using EveSquadron.DataAccess.Interfaces;
using EveSquadron.Models;
using EveSquadron.Models.EVE.Data;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json.Linq;

namespace EveSquadron.DataAccess;

public class EveRestDataAccess : RestDataAccessBase, IEveRestDataAccess
{
    #region member fields

    private readonly HttpClient _httpClient;
    private readonly ILogger<EveRestDataAccess> _logger;

    #endregion

    #region constructor

    public EveRestDataAccess(HttpClient httpClient,
        ILogger<EveRestDataAccess> logger) : base(httpClient, logger)
    {
        _httpClient = httpClient;
        _logger = logger;
    }

    #endregion

    #region interface implementation

    public async Task<T> GetIDsFromNames<T>(string idEndpoint, IEnumerable<string> names)
    {
        var inputNames = names.ToArray();
        _logger.LogDebug($"Loading Character IDs from endpoint '{idEndpoint}' for {inputNames.Length} characters.");

        var namesLookupLimit = AppConstants.EveAPILimits.PostUniverseIDsPlayerCountLimit;
        var results = new List<JObject>();

        for (var i = 0; i < inputNames.Length; i += namesLookupLimit)
        {
            var lookupNames = inputNames.Skip(i).Take(namesLookupLimit);

            var payload = new StringContent(JsonSerializer.Serialize(lookupNames));
            var response = await CreateAsync<T>(idEndpoint, payload);
            var responseJson = JsonSerializer.Serialize(response);
            results.Add(JObject.Parse(responseJson));
        }

        var resultObject = new JObject();
        foreach (var resultPart in results)
        {
            resultObject.Merge(resultPart);
        }

        return JsonSerializer.Deserialize<T>(resultObject.ToString(), AppConstants.AppDefaultSerializerOptions)!;
    }

    public Task<byte[]> GetPortraitByteArrayAsync(string portraitEndpoint)
    {
        _logger.LogDebug($"Downloading image from endpoint '{portraitEndpoint}'");
        return _httpClient.GetByteArrayAsync(portraitEndpoint);
    }

    public async Task<IEnumerable<T>> GetNamesFromIDs<T>(string namesEndpoint, IEnumerable<int> ids)
    {
        var inputIDs = ids.ToArray();
        _logger.LogDebug($"Loading CharacterNames from endpoint '{namesEndpoint}' for {inputIDs.Length} IDs.");
        var idsLookupLimit = AppConstants.EveAPILimits.PostUniverseNamesIDsLimit;

        var results = new List<JArray>();

        for (var i = 0; i < inputIDs.Length; i += idsLookupLimit)
        {
            var lookupIDs = inputIDs.Skip(i).Take(idsLookupLimit);
            var payload = new StringContent(JsonSerializer.Serialize(lookupIDs));

            var response = await GetAllPOSTAsync<EveNameLookup>(namesEndpoint, payload);
            var responseJson = JsonSerializer.Serialize(response);
            results.Add(JArray.Parse(responseJson));
        }

        var resultObject = new JArray();
        foreach (var resultPart in results)
        {
            resultObject.Merge(resultPart);
        }

        return JsonSerializer.Deserialize<IEnumerable<T>>(resultObject.ToString(), AppConstants.AppDefaultSerializerOptions)!;
    }

    public Task<T> GetDetailedKillInformationFor<T>(string killmailsEndpoint, string killmailHash, int killmailID)
    {
        _logger.LogDebug($"GetDetailedKillInformationFor on endpoint '{killmailsEndpoint}' for killmail hash '{killmailHash}' and killmail id '{killmailID}'.");
        return GetByIdAsync<T>($"{killmailsEndpoint}{killmailID}/{killmailHash}");
    }
    
    public Task<T> GetCharacterInformationFor<T>(string characterEndpoint, int playerID)
    {
        _logger.LogDebug($"GetCharacterInformationFor on endpoint '{characterEndpoint}' for player id '{playerID}'.");
        return GetByIdAsync<T>($"{characterEndpoint}{playerID}");
    }
    
    #endregion
}