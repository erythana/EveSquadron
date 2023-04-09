using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;
using Avalonia.Media.Imaging;
using EVEye.DataAccess.Interfaces;
using EVEye.Models.EVE.Data;
using EVEye.Models.EVE.Interfaces;
using EVEye.Models.Interfaces;
using Microsoft.Extensions.Logging;

namespace EVEye.Models.EVE
{
    public class EveDataRepository : IEveDataRepository
    {
        #region member fields

        private readonly IEveRestDataAccess _eveRestDataAccess;
        private readonly ILogger<EveDataRepository> _logger;

        private readonly string _characterEndpoint;
        private readonly string _portraitEndpoint;

        #endregion

        #region constructor
        
        public EveDataRepository(IEveRestDataAccess eveRestDataAccess, IEveESIEndpointsLoader endpoints, ILogger<EveDataRepository> logger)
        {
            _eveRestDataAccess = eveRestDataAccess;
            _logger = logger;

            _characterEndpoint = endpoints.UniverseEndpoint;
            _portraitEndpoint = endpoints.PortraitEndpoint;
        }

        #endregion

        #region interface methods
        
        public Task<EveUniverseIDMapping> GetIDsFrom(IEnumerable<string> names)
        {
            return _eveRestDataAccess.GetCharacterIDsFromNames<EveUniverseIDMapping>(_characterEndpoint, names);
        }
        
        public async Task<Bitmap?> GetPortraitFrom(int characterID, int width)
        {
            var imageContent = await _eveRestDataAccess.GetPortraitByteArrayAsync(_portraitEndpoint + characterID + $"/portrait?size={width}");
            var stream = new MemoryStream(imageContent);
            return Bitmap.DecodeToWidth(stream, width);
        }
        
        #endregion
    }
}