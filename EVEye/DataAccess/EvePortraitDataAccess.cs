using System.Net.Http;
using System.Threading.Tasks;
using EVEye.DataAccess.Interfaces;
using Microsoft.Extensions.Logging;

namespace EVEye.DataAccess
{
    public class EvePortraitDataAccess : IEvePortraitDataAccess
    {
        private readonly HttpClient _httpClient;
        private readonly ILogger<EvePortraitDataAccess> _logger;

        public EvePortraitDataAccess(HttpClient httpClient, ILogger<EvePortraitDataAccess> logger)
        {
            _httpClient = httpClient;
            _logger = logger;
        }
        
        public Task<byte[]> GetPortraitByteArrayAsync(string endpoint)
        {
            _logger.LogDebug($"Trying to download image from {endpoint}");
            return _httpClient.GetByteArrayAsync(endpoint);
        }
    }

}