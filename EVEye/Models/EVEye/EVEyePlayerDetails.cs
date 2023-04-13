using System;
using System.Threading.Tasks;
using EVEye.Models.EVE.Data;

namespace EVEye.Models.EVEye;

public class EVEyePlayerDetails : ModelBase
{
    #region member fields
    
    private int _id;
    private DateTime _birthdate;
    private int _shipsDestroyed;
    private int _shipsLost;
    private int _dangerRatio;
    private int _gangRatio;
    private int _soloKills;
    private int _soloLosses;
    private Task<EVEyeKillInformation>? _latestKillboardActivity;

    #endregion
    
    #region properties
    
    public int ID {
        get => _id;
        set => SetProperty(ref _id, value);
    }

    public DateTime Birthdate {
        get => _birthdate;
        set => SetProperty(ref _birthdate, value);
    }

    public int ShipsDestroyed {
        get => _shipsDestroyed;
        set => SetProperty(ref _shipsDestroyed, value);
    }

    public int ShipsLost {
        get => _shipsLost;
        set => SetProperty(ref _shipsLost, value);
    }

    public int DangerRatio {
        get => _dangerRatio;
        set => SetProperty(ref _dangerRatio, value);
    }

    public int GangRatio {
        get => _gangRatio;
        set => SetProperty(ref _gangRatio, value);
    }

    public int SoloKills {
        get => _soloKills;
        set
        {
            SetProperty(ref _soloKills, value);
            RaisePropertyChanged(nameof(SoloDangerRatio));
        }
    }

    public int SoloLosses {
        get => _soloLosses;
        set
        {
            SetProperty(ref _soloLosses, value);
            RaisePropertyChanged(nameof(SoloDangerRatio));
        }
    }

    public int SoloDangerRatio { get => GetSoloDangerRatio(); }

    public Task<EVEyeKillInformation>? LatestKillboardActivity {
        get => _latestKillboardActivity;
        set => SetProperty(ref _latestKillboardActivity, value);
    }
    //public Task<EveDetailedKillInformation?>? LatestKill { get; set; }

    #endregion

    #region helper methods
    
    private int GetSoloDangerRatio()
    {
        var dangerRatio = SoloKills == 0 ? 0
            : SoloLosses == 0 ? 100
            : 100 * (float)SoloLosses / SoloKills;
        return (int)dangerRatio;
    }
    
    #endregion

}