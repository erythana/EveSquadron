using System.Collections.Generic;
using System.Threading.Tasks;
using Avalonia.Media.Imaging;
using EVEye.Models.EVE.Data;

namespace EVEye.Models.EVE.Interfaces
{
    public interface IEveDataRepository
    {
        public Task<EveUniverseIDMapping> GetIDsFrom(IEnumerable<string> names);
        
        public Task<Bitmap?> GetPortraitFrom(int characterID, int width);
    }
}