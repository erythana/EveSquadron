using System.Collections.Generic;
using System.Threading.Tasks;
using EVEye.Models.EVEye;

namespace EVEye.DataAccess.Interfaces;

public interface IPlayerInformationDataAggregator
{
    public IAsyncEnumerable<EVEyePlayerInformation> GetAggregatedItemsFor(IEnumerable<string> players);
}