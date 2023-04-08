using System.Collections.Generic;
using System.Threading.Tasks;
using EVEye.DataAccess.Interfaces;
using EVEye.Models.EVE.Data;
using EVEye.Models.EVE.Interfaces;
using EVEye.Models.Interfaces;
using Microsoft.Extensions.Logging;

namespace EVEye.Models.EVE
{
    public class EveDataRepository : IEveDataRepository
    {
        private readonly IEveRestDataAccess<EveUniverseIDMapping> _eveRestDataAccess;
        private readonly ILogger<EveDataRepository> _logger;

        private readonly string _characterEndpoint;

        public EveDataRepository(IEveRestDataAccess<EveUniverseIDMapping> eveRestDataAccess, IEveESIEndpointsLoader endpoints, ILogger<EveDataRepository> logger)
        {
            _eveRestDataAccess = eveRestDataAccess;
            _logger = logger;

            _characterEndpoint = endpoints.CharacterEndpoint;
        }

        public Task<EveUniverseIDMapping> GetIDsFrom(IEnumerable<string> names)
        {
            return _eveRestDataAccess.GetCharacterIDsFromNames(_characterEndpoint, names);
        }
    }
}