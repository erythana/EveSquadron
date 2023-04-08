using System.Collections.Generic;
using System.Threading.Tasks;

namespace EVEye.DataAccess.Interfaces
{
    public interface IEveRestDataAccess<T>
    {
        public Task<T> GetCharacterIDsFromNames(string endpoint, IEnumerable<string> names);
    }
}