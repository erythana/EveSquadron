namespace EVEye.Models.ZKillboard.Data
{
    public class ZKillboardCharacterStatistic
    {
        public int ID { get; set; }
        public int ShipsDestroyed { get; set; }
        public int ShipsLost { get; set; }
        public int DangerRatio { get; set; }
        public int GangRatio { get; set; }
        public int SoloKills { get; set; }
        public int SoloLosses { get; set; }
        public int SoloDangerRatio { get => SoloKills / 100 * SoloLosses; }//TODO: Check and change Datatype
        public ZKillboardPlayerInfo Info { get; set; }
    }
}