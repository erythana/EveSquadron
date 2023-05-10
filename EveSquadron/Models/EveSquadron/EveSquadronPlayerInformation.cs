using System;
using System.Threading.Tasks;
using Avalonia.Media.Imaging;
using EveSquadron.Models.EVE.Data;

namespace EveSquadron.Models.EveSquadron;

// ReSharper disable once InconsistentNaming
public class EveSquadronPlayerInformation : ModelBase
{
    #region member fields

    private int _id;
    private EveNameIDMapping _character;
    private DateTime? _birthday;
    private Task<Bitmap?> _characterImage;
    private float _securityStanding;
    private EveNameIDMapping? _corporation;
    private EveNameIDMapping? _alliance;
    private Lazy<Task<EveSquadronPlayerDetails>> _playerDetails;
    private int _corporationPasteCount;
    private int _alliancePasteCount;

    #endregion
    
    #region properties

    public int ID {
        get => _id;
        set => SetProperty(ref _id, value);
    }

    public EveNameIDMapping Character {
        get => _character;
        set => SetProperty(ref _character, value);
    }

    public DateTime? Birthday {
        get => _birthday;
        set => SetProperty(ref _birthday, value);
    }

    public Task<Bitmap?> CharacterImage {
        get => _characterImage;
        set => SetProperty(ref _characterImage, value);
    }

    public float SecurityStanding {
        get => _securityStanding;
        set => SetProperty(ref _securityStanding, value);
    }

    public EveNameIDMapping? Corporation {
        get => _corporation;
        set => SetProperty(ref _corporation, value);
    }
    
    public int CorporationPasteCount {
        get => _corporationPasteCount;
        set => SetProperty(ref _corporationPasteCount, value);
    }

    public EveNameIDMapping? Alliance {
        get => _alliance;
        set => SetProperty(ref _alliance, value);
    }
    
    public int AlliancePasteCount {
        get => _alliancePasteCount;
        set => SetProperty(ref _alliancePasteCount, value);
    }

    public Lazy<Task<EveSquadronPlayerDetails>> PlayerDetails {
        get => _playerDetails;
        set => SetProperty(ref _playerDetails, value);
    }
    
    #endregion
}