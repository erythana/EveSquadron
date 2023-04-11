namespace EVEye.Models.ZKillboard.Data;

public class ZKillboardCharacterStatistic
{
    public int ID { get; set; }
    public int ShipsDestroyed { get; set; }
    public int ShipsLost { get; set; }
    public int DangerRatio { get; set; }
    public int GangRatio { get; set; }
    public int SoloKills { get; set; }
    public int SoloLosses { get; set; }
    public ZKillboardPlayerInfo? Info { get; set; }
}