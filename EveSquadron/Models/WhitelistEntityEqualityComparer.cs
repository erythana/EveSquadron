using System;
using System.Collections.Generic;
using EveSquadron.Models.EveSquadron.Interfaces;

namespace EveSquadron.Models;

public class WhitelistEntityEqualityComparer : IEqualityComparer<IWhitelistEntry>
{
    public bool Equals(IWhitelistEntry? x, IWhitelistEntry? y)
    {
        if (ReferenceEquals(x, y))
            return true;
        if (ReferenceEquals(x, null))
            return false;
        if (ReferenceEquals(y, null))
            return false;
        if (x.GetType() != y.GetType())
            return false;
        return x.Type == y.Type && x.Name == y.Name;
    }

    public int GetHashCode(IWhitelistEntry obj)
    {
        return HashCode.Combine((int)obj.Type, obj.Name);
    }
}