namespace EveSquadron.Models.EveSquadron.Interfaces;

public interface IWhitelistEntry
{
    public EntityTypeEnum Type { get; set; }

    public string Name { get; set; }
}