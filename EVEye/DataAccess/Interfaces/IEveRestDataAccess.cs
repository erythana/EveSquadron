using System.Collections.Generic;
using System.Threading.Tasks;

namespace EVEye.DataAccess.Interfaces
{
    public interface IEveRestDataAccess
    {
        public Task<T> GetCharacterIDsFromNames<T>(string endpoint, IEnumerable<string> names);
        public Task<byte[]> GetPortraitByteArrayAsync(string endpoint);
    }
}