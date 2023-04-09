using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text.Json;
using System.Threading.Tasks;
using EVEye.DataAccess.Base;
using EVEye.DataAccess.Interfaces;
using EVEye.Models;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json.Linq;

namespace EVEye.DataAccess
{
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
        
        public async Task<T> GetCharacterIDsFromNames<T>(string endpoint, IEnumerable<string> names)
        {
            var namesLookupLimit = ApplicationConstants.EveAPILimits.PostUniverseIDsCharactersLimit;
            var inputNames = names.ToArray();
    
            var results = new List<JObject>();
            _logger.LogDebug($"Loading Character IDs from endpoint {endpoint} for {inputNames.Length} characters.");
            
            for (var i = 0; i < inputNames.Length; i += namesLookupLimit)
            {
                var lookupNames = inputNames.Skip(i).Take(namesLookupLimit);
                
                var payload = new StringContent(JsonSerializer.Serialize(lookupNames));
                var response = await CreateAsync<T>(endpoint, payload);
                var responseJson = JsonSerializer.Serialize(response);
                results.Add(JObject.Parse(responseJson));
            }

            var resultObject = new JObject();
            foreach (var resultPart in results)
                resultObject.Merge(resultPart);

            return JsonSerializer.Deserialize<T>(resultObject.ToString(), ApplicationConstants.AppDefaultSerializerOptions)!;
        }

        public Task<byte[]> GetPortraitByteArrayAsync(string endpoint)
        {
            _logger.LogDebug($"Trying to download image from {endpoint}");
            return _httpClient.GetByteArrayAsync(endpoint);
        }
        
        #endregion
    }

}