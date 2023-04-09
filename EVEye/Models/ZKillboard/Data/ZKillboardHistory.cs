using Newtonsoft.Json;

namespace EVEye.Models.ZKillboard.Data
{
    public class ZKillboardHistory
    {
        [JsonProperty("Killmail_ID")]
        public int ID { get; set; }

        [JsonProperty("ZKB")]
        public ZKillboardEntry ZKillboardEntry { get; set; }
    }

}