using System.Collections.Generic;
using System.Threading.Tasks;
using EVEye.Models;

namespace EVEye.DataAccess.Interfaces
{
    public interface IPlayerInformationDataAggregator
    {
        public Task<IEnumerable<EVEyePlayerInformation>> GetAggregatedItemsFor(IEnumerable<string> players);
    }
}