using System;

namespace EveSquadron.Models.ZKillboard.Data;

public class ZKillboardPlayerInfo
{
    public int ID { get; set; }
    public string Name { get; set; }
    public int CorporationID { get; set; }
    public int AllianceID { get; set; }
    
    public DateTime Birthday { get; set; }
    public double SecStatus { get; set; }

    public LastAPIUpdate LastApiUpdate { get; set; }
}