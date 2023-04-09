namespace EVEye.Models.EVEye;

public class EVEyePlayerDetails
{
    public int? ID { get; set; }
    public int ShipsDestroyed { get; set; }
    public int ShipsLost { get; set; }
    public int DangerRatio { get; set; }
    public int GangRatio { get; set; }
    public int SoloKills { get; set; }
    public int SoloLosses { get; set; }
    public int SoloDangerRatio { get => GetSoloDangerRatio(); }
    
    private int GetSoloDangerRatio()
    {
        var dangerRatio = SoloKills == 0 ? 0
            : SoloLosses == 0 ? 100
            : 100 * (float)SoloLosses / SoloKills;
        return (int)dangerRatio;
    }
}