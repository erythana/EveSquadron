using System;
using System.Threading.Tasks;

namespace EVEye.Models.EVEye;

public class EVEyeKillInformation
{
    public string AttackerShip { get; set; }
    public string AttackerGuns { get; set; }
    public string VictimShip { get; set; }
    public string SolarSystem { get; set; }
    public DateTime Date { get; set; }
}