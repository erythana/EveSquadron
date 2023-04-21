using System.Text.Json.Serialization;

namespace EveSquadron.Models.EVE.Data
{
    public class EveKillAttacker
    {
        [JsonPropertyName("Character_ID")]
        public int ID { get; set; }
        [JsonPropertyName("Final_Blow")]
        public bool FinalBlow { get; set; }
        [JsonPropertyName("Ship_Type_ID")]
        public int ShipTypeID { get; set; }
        [JsonPropertyName("Weapon_Type_ID")]
        public int WeaponTypeID { get; set; }
    }
}