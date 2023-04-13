using System.Collections.Generic;
using System.Threading.Tasks;
using EVEye.Models.EVE.Data;
using EVEye.Models.ZKillboard.Data;

namespace EVEye.Models.EVEye;

public class AggregatorInformation
{
    public EVEyePlayerInformation EVEyePlayerInformation { get; set; }
    public Task<EveCharacter> EveCharacter { get; set; }
    public Task<ZKillboardCharacterStatistic> ZKillboardCharacterStatistic { get; set; }
    public Task<IEnumerable<ZKillboardEntry>> KillboardHistory { get; set; }

}