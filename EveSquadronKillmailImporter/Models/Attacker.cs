namespace EveSquadronKillmailImporter.Models;

public record Attacker
{
    public int? character_id { get; set; }
    public bool final_blow { get; set; }
    public int? ship_type_id { get; set; }
    public int? weapon_type_id { get; set; }
}