using System;
using System.Collections.Generic;
using EveSquadron.Models.EveSquadron;

namespace EveSquadron.Models.Interfaces;

public interface IPlayerInformationDataAggregator
{
    public IAsyncEnumerable<EveSquadronPlayerInformation> GetAggregatedItemsFor(IEnumerable<string> players);
    public event EventHandler<(int? CorporationID, int? AllianceID)> ParsedNewID;
    public event EventHandler OnValidPaste;
}