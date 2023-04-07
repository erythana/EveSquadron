namespace EVEye.Models.ZKillboard.Data
{
    public class KillboardEntry
    {
        public int LocationID { get; set; }
        public string Hash { get; set; }
        public long FittedValue { get; set; }
        public long DroppedValue { get; set; }
        public long DestroyedValue { get; set; }
        public long TotalValue { get; set; }
        public int Points { get; set; }
        public bool NPC { get; set; }
        public bool Solo { get; set; }
        public bool Awox { get; set; }
    }
}