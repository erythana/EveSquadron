using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text.Json;
using System.Text.Json.Nodes;
using System.Threading.Tasks;
using EVEye.DataAccess.Base;
using EVEye.DataAccess.Interfaces;
using EVEye.Models;
using EVEye.Models.EVE.Data;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json.Linq;

namespace EVEye.DataAccess
{
    public class EveRestDataAccess<T> : RestDataAccessBase<T>, IEveRestDataAccess<T>
    {
        private readonly HttpClient _httpClient;
        private readonly ILogger<EveRestDataAccess<T>> _logger;
    
        public EveRestDataAccess(HttpClient httpClient,
            ILogger<EveRestDataAccess<T>> logger) : base(httpClient, logger)
        {
            _httpClient = httpClient;
            _logger = logger;
        }
        
        public async Task<T> GetCharacterIDsFromNames(string endpoint, IEnumerable<string> names)
        {
            var namesLookupLimit = ApplicationConstants.EveAPILimits.PostUniverseIDsCharactersLimit;
            var inputNames = names.ToArray();
    
            var results = new List<JObject>();
            _logger.LogDebug($"Loading Character IDs from endpoint {endpoint} for {inputNames.Length} characters.");
            
            for (var i = 0; i < inputNames.Count(); i += namesLookupLimit)
            {
                var lookupNamesJson = JsonSerializer.Serialize(inputNames.Skip(i).Take(namesLookupLimit));
                var content = new StringContent(lookupNamesJson);
                var resultPart = await GetJsonPostAsync(endpoint, content);
                results.Add(JObject.Parse(resultPart));
            }

            var resultObject = new JObject();
            foreach (var resultPart in results)
                resultObject.Merge(resultPart);

            return JsonSerializer.Deserialize<T>(resultObject.ToString(), new JsonSerializerOptions()
            {
                PropertyNameCaseInsensitive = true
            })!;
        }
    
        #region helper methods
    
        //TODO: Maybe put a post method like this into the base layer
        private async Task<string> GetJsonPostAsync(string endpointUrl, HttpContent content)
        {
            var response = await _httpClient.PostAsync(endpointUrl, content);
            response.EnsureSuccessStatusCode();
            
            return await response.Content.ReadAsStringAsync();
        }
    
        #endregion
    }

}