namespace EveSquadronKillmailImporter.Models;

public record EveKillmail
{
        public int killmail_id { get; set; }
        public List<Attacker> attackers { get; set; }
        public DateTime killmail_time { get; set; }
        public int solar_system_id { get; set; }
        public Victim victim { get; set; }
        public string killmail_hash { get; set; }
}