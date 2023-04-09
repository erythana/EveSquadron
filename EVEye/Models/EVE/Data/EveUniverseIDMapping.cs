using System.Collections.Generic;

namespace EVEye.Models.EVE.Data
{
    public class EveUniverseIDMapping
    {
        public List<EveNameIDMapping> Characters { get; set; } = new List<EveNameIDMapping>();
        public List<EveNameIDMapping> Agents { get; set; } = new List<EveNameIDMapping>();
        public List<EveNameIDMapping> Alliances { get; set; } = new List<EveNameIDMapping>();
        public List<EveNameIDMapping> Constellations { get; set; } = new List<EveNameIDMapping>();
        public List<EveNameIDMapping> Corporations { get; set; } = new List<EveNameIDMapping>();
        public List<EveNameIDMapping> Factions { get; set; } = new List<EveNameIDMapping>();
        public List<EveNameIDMapping> Inventory_Types { get; set; } = new List<EveNameIDMapping>();
        public List<EveNameIDMapping> Regions { get; set; } = new List<EveNameIDMapping>();
        public List<EveNameIDMapping> Stations { get; set; } = new List<EveNameIDMapping>();
        public List<EveNameIDMapping> Systems { get; set; } = new List<EveNameIDMapping>();
    }
}