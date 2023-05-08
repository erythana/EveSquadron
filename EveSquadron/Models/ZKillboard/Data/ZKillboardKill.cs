namespace EveSquadron.Models.ZKillboard.Data;

public class ZKillboardKill
{
    public int LocationID { get; set; }
    public string Hash { get; set; }
    public double FittedValue { get; set; }
    public double DroppedValue { get; set; }
    public double DestroyedValue { get; set; }
    public double TotalValue { get; set; }
    public int Points { get; set; }
    public bool NPC { get; set; }
    public bool Solo { get; set; }
    public bool Awox { get; set; }
}