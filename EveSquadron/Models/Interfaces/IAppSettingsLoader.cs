namespace EveSquadron.Models.Interfaces;

public interface IAppSettingsLoader
{
    public string ClipboardPollingMilliseconds { get; set; }
    public string Theme { get; set; }
    public string HoverColor { get; set; }
}