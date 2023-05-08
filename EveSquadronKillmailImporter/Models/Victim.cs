namespace EveSquadronKillmailImporter.Models;

public record Victim
{
    public int? character_id { get; set; }
    public int ship_type_id { get; set; }
}