using System.Collections.Generic;

namespace EveSquadron.Models.Interfaces;

public interface IClipboardToWhitelistEntitiesParser<out T> where T : IWhitelistEntry
{
    public IEnumerable<T> Parse(string clipboardContent);
}