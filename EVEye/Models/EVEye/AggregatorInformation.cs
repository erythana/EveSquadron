using System.Collections.Generic;
using EVEye.Models.ZKillboard.Data;

namespace EVEye.Models.EVEye;

public class AggregatorInformation
{
    public EVEyePlayerInformation EVEyePlayerInformation { get; set; }
    public ZKillboardCharacterStatistic ZKillboardCharacterStatistic { get; set; }
    public IEnumerable<ZKillboardEntry>? KillboardHistory { get; set; }

}