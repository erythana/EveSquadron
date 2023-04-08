using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;
using Avalonia.Controls;
using Avalonia.Media.Imaging;
using EVEye.DataAccess;
using EVEye.DataAccess.Interfaces;
using EVEye.Extensions;
using EVEye.Models.EVE.Data;
using EVEye.Models.EVE.Interfaces;
using EVEye.Models.Interfaces;
using Microsoft.Extensions.Logging;

namespace EVEye.Models.EVE
{
    public class EveDataRepository : IEveDataRepository
    {
        private readonly IEveRestDataAccess<EveUniverseIDMapping> _universeIDMappingDataAccess;
        private readonly IEvePortraitDataAccess _evePortraitDataAccess;
        private readonly ILogger<EveDataRepository> _logger;

        private readonly string _characterEndpoint;
        private readonly string _portraitEndpoint;

        public EveDataRepository(IEveRestDataAccess<EveUniverseIDMapping> universeIDMappingDataAccess, IEvePortraitDataAccess evePortraitDataAccess, IEveESIEndpointsLoader endpoints, ILogger<EveDataRepository> logger)
        {
            _universeIDMappingDataAccess = universeIDMappingDataAccess;
            _evePortraitDataAccess = evePortraitDataAccess;
            _logger = logger;

            _characterEndpoint = endpoints.UniverseEndpoint;
            _portraitEndpoint = endpoints.PortraitEndpoint;
        }

        public Task<EveUniverseIDMapping> GetIDsFrom(IEnumerable<string> names)
        {
            return _universeIDMappingDataAccess.GetCharacterIDsFromNames(_characterEndpoint, names);
        }
        public async Task<Bitmap?> GetPortraitFrom(int characterID, int width)
        {
            var imageContent = await _evePortraitDataAccess.GetPortraitByteArrayAsync(_portraitEndpoint + characterID + $"/portrait?size={width}");
            var stream = new MemoryStream(imageContent);
            return Bitmap.DecodeToWidth(stream, width);
        }
    }
}