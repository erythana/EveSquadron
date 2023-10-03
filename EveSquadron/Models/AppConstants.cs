using System.Text.Json;
using Avalonia.Media;
using EveSquadron.Models.Enums;

namespace EveSquadron.Models;

public static class AppConstants
{
    public const string ApplicationName = "Eve Squadron";
    public const string UserAgentHeader = $"{ApplicationName} - erythanadevsup@gmail.com";

    #region Setting Defaults

    public const int MinimalClipboardPollingMs = 100;
    public const int MaximalClipboardPollingMs = 20000;
    public const int DefaultClipboardPollingMs = 250;
    public static Color DefaultHoverColor = Colors.Orange;
    public static GridRowSizeEnum DefaultGridRowSize = GridRowSizeEnum.Large;

    #endregion

    #region Serialization options
    
    public static readonly JsonSerializerOptions AppDefaultSerializerOptions = new()
    {
        WriteIndented = true,
        PropertyNameCaseInsensitive = true
    };

    #endregion

    #region ZKillboard Urls

    public static class ZKillboardUrls
    {
        public const string Character = "https://zkillboard.com/character";
        public const string Corporation = "https://zkillboard.com/corporation";
        public const string Alliance = "https://zkillboard.com/alliance";
    }

    #endregion
    
    #region API Limits
    
    public static class ZKillboardAPILimits
    {
        public const int ZKillboardRateLimitMs = 200;
    }

    public static class EveAPILimits
    {
        public const int PostUniverseIDsNameCharacterLimit = 100;
        public const int PostUniverseIDsPlayerCountLimit = 500;
        public const int PostUniverseNamesIDsLimit = 1000;
    }
    
    #endregion

    #region DB Defaults

    public const string SettingsDatabase = "EveSquadron.Database.sqlite";
    public static string GetLocalConnectionString(string databaseFile) => $"Data Source={databaseFile};Version=3;";

    #endregion
}
