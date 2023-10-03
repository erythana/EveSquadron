using EveSquadron.Models.Enums;
using EveSquadron.Models.Interfaces;

namespace EveSquadron.Models;

public class WhitelistEntry : ModelBase, IWhitelistEntry
{
    private EntityTypeEnum _type;
    private string _name = string.Empty;

    public EntityTypeEnum Type {
        get => _type;
        set => SetProperty(ref _type, value);
    }

    public string Name {
        get => _name;
        set => SetProperty(ref _name, value);
    }
}