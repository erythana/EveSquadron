using System.Collections.Generic;
// ReSharper disable All

namespace EveSquadron.Models.EVE.Data;

public class EveUniverseIDMapping
{
    public List<EveNameIDMapping> Characters { get; set; } = new();
    public List<EveNameIDMapping> Agents { get; set; } = new();
    public List<EveNameIDMapping> Alliances { get; set; } = new();
    public List<EveNameIDMapping> Constellations { get; set; } = new();
    public List<EveNameIDMapping> Corporations { get; set; } = new();
    public List<EveNameIDMapping> Factions { get; set; } = new();
    public List<EveNameIDMapping> Inventory_Types { get; set; } = new();
    public List<EveNameIDMapping> Regions { get; set; } = new();
    public List<EveNameIDMapping> Stations { get; set; } = new();
    public List<EveNameIDMapping> Systems { get; set; } = new();
}