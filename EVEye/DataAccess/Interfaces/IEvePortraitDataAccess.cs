using System.Threading.Tasks;

namespace EVEye.DataAccess.Interfaces
{
    public interface IEvePortraitDataAccess
    {
        public Task<byte[]> GetPortraitByteArrayAsync(string endpoint);
    }
}