using EveSquadron.Models.Enums;

namespace EveSquadron.Models.Interfaces;

public interface IWhitelistEntry
{
    public EntityTypeEnum Type { get; set; }

    public string Name { get; set; }
}