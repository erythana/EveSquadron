using Dapper.Contrib.Extensions;

namespace EveSquadronKillmailImporter.Models;

[Table("Killmail")]
public class Killmail
{
    [ExplicitKey]
    public int ID { get; set; }
    public string KillmailHash { get; set;}
    public int SolarSystemID { get; set; }
    public int? LossCharacterID { get; set;}
    public int LossShipTypeID { get; set;}
    public int? WinCharacterID { get; set;}
    public int? WinShipTypeID { get; set;}
    public int? WinWeaponTypeID { get; set;}
    public int WinAttackerCount { get; set;}
    public DateTime KillmailDate { get; set;}
}