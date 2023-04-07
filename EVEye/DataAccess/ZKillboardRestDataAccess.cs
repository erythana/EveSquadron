using System.Collections.Generic;
using System.Net.Http;
using System.Threading.Tasks;
using EVEye.DataAccess.Base;
using EVEye.DataAccess.Interfaces;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;

namespace EVEye.DataAccess
{
    public class ZKillboardRestDataAccess<T> : RestDataAccessBase<T>, IZkillboardRestDataAccess
    {
        private readonly ILogger<ZKillboardRestDataAccess<T>> _logger;

        public ZKillboardRestDataAccess(HttpClient httpClient,
            IConfiguration configuration,
            ILogger<ZKillboardRestDataAccess<T>> logger) : base(httpClient)
        {
            _logger = logger;
        }

        public async Task<T?> GetKillmailsFrom(string endpoint, int id)
        {
            _logger.LogDebug($"Loading Killmails from endpoint {endpoint} for ID {id}");

            var result = await GetByIdAsync("thekillmailendpoint", id);
            return result;
        }
    }

}