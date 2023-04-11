namespace EVEye.Models.Interfaces;

public interface IAppSettingsLoader
{
    public string ClipboardPollingMilliseconds { get; set; }
    public string Theme { get; set; }
}