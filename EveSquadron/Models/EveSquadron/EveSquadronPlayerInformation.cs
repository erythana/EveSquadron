using System;
using System.Threading.Tasks;
using Avalonia.Media.Imaging;

namespace EveSquadron.Models.EveSquadron;

// ReSharper disable once InconsistentNaming
public class EveSquadronPlayerInformation : ModelBase
{
    #region member fields

    private int _id;
    private string _characterName;
    private DateTime? _birthday;
    private Task<Bitmap?> _characterImage;
    private float _securityStanding;
    private string? _corporationName;
    private string? _allianceName;
    private Lazy<Task<EveSquadronPlayerDetails>> _playerDetails;
    
    #endregion
    
    #region properties

    public int ID {
        get => _id;
        set => SetProperty(ref _id, value);
    }

    public string CharacterName {
        get => _characterName;
        set => SetProperty(ref _characterName, value);
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

    public string? CorporationName {
        get => _corporationName;
        set => SetProperty(ref _corporationName, value);
    }

    public string? AllianceName {
        get => _allianceName;
        set => SetProperty(ref _allianceName, value);
    }

    public Lazy<Task<EveSquadronPlayerDetails>> PlayerDetails {
        get => _playerDetails;
        set => SetProperty(ref _playerDetails, value);
        
    }
    
    #endregion
}