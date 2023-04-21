using System.Text.Json.Serialization;
using Newtonsoft.Json;

namespace EveSquadron.Models.EVE.Data
{
    public class EveKillVictim
    {
        [JsonPropertyName("Character_ID")]
        public int ID { get; set; }
        
        [JsonPropertyName("Ship_Type_ID")]
        public int ShipTypeID { get; set; }
    }
}