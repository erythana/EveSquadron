using System;
using System.Collections.Generic;
using System.Text.Json.Serialization;
using Newtonsoft.Json;

namespace EVEye.Models.EVE.Data
{
    public class EveDetailedKillInformation
    {
        [JsonPropertyName("killmail_id")]
        public int ID { get; set; }
        public List<EveKillAttacker> Attackers { get; set; }
        [JsonPropertyName("Killmail_Time")]
        public DateTime KillDate { get; set; }
        [JsonPropertyName("Solar_System_ID")]
        public int SolarSystemID { get; set; }
        public EveKillVictim Victim { get; set; }
    }
}