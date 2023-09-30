using System.Collections.Generic;
using EveSquadron.Models.EveSquadron.Interfaces;

namespace EveSquadron.Models.Interfaces;

public interface IClipboardToWhitelistEntitiesParser<T> where T : IWhitelistEntry
{
    public IEnumerable<T> Parse(string clipboardContent);
}