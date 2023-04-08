using System.Collections.Generic;

namespace EVEye.Models.EVE.Data
{
    public struct EveUniverseIDMapping
    {
        public List<EveNameIDMapping> Characters { get; set; }
        
        // Keep those ready in case we need them during next development phase
        // public List<EveNameIDMapping> Agents { get; set; }
        // public List<EveNameIDMapping> Alliances { get; set; }
        // public List<EveNameIDMapping> Constellations { get; set; }
        // public List<EveNameIDMapping> Corporations { get; set; }
        // public List<EveNameIDMapping> Factions { get; set; }
        // public List<EveNameIDMapping> Inventory_types { get; set; }
        // public List<EveNameIDMapping> Regions { get; set; }
        // public List<EveNameIDMapping> Stations { get; set; }
        // public List<EveNameIDMapping> Systems { get; set; }
    }
}