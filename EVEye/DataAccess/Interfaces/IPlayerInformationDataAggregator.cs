using System.Collections.Generic;
using System.Threading.Tasks;
using EVEye.Models.EVEye;

namespace EVEye.DataAccess.Interfaces;

public interface IPlayerInformationDataAggregator
{
    public Task<IEnumerable<EVEyePlayerInformation>> GetAggregatedItemsFor(IEnumerable<string> players);
}