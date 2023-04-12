using System;
using System.Threading.Tasks;

namespace EVEye.Models.EVEye;

public class EVEyeKillInformation
{
    public Task<string> AttackerShip { get; set; }
    public Task<string> AttackerGuns { get; set; }
    public Task<string> VictimShip { get; set; }
    public Task<string> SolarSystem { get; set; }
    
    public DateTime Date { get; set; }
}