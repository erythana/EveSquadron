using System;
using System.Threading.Tasks;
using Avalonia.Media.Imaging;

namespace EVEye.Models.EVEye;

// ReSharper disable once InconsistentNaming
public class EVEyePlayerInformation
{
    public int ID { get; set; }
    public string CharacterName { get; set; }
    public DateTime? Birthday { get; set; }
    public Task<Bitmap?> CharacterImage { get; set; }
    public float SecurityStanding { get; set; }
    public string CorporationName { get; set; }
    public string? AllianceName { get; set; }
    public EVEyePlayerDetails PlayerDetails { get; set; }
}