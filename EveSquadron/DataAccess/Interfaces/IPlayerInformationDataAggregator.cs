using System.Collections.Generic;
using EveSquadron.Models.EveSquadron;

namespace EveSquadron.DataAccess.Interfaces;

public interface IPlayerInformationDataAggregator
{
    public IAsyncEnumerable<EveSquadronPlayerInformation> GetAggregatedItemsFor(IEnumerable<string> players);
}