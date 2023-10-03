namespace EveSquadron.Models.Options;

public class EveSquadronOptions
{
        public const string Section = "EveSquadronOptions";
        
        public string Theme { get; set; }
        public string ClipboardPollingMilliseconds { get; set; }
        public string HoverColor { get; set;}
        public string ShowPortrait { get; set;}
        public string GridRowSize { get; set;}
        
        public string AutoExport { get; set;}
        
        public string AutoExportFile { get; set;}
}
