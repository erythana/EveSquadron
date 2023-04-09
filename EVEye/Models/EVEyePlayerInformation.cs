using Avalonia.Media.Imaging;

namespace EVEye.Models
{
    // ReSharper disable once InconsistentNaming
    public class EVEyePlayerInformation
    {
        public Bitmap? CharacterImage { get; set; }
        public int? ID { get; set; }
        public string CharacterName { get; set; }
        public double? SecurityStanding { get; set; }
        public string CorporationName { get; set; }
        public string AllianceName { get; set; }
    }
}