using System;
using System.IO;
using System.Text.Json;
using Avalonia.Media;
using EveSquadron.Models.Enums;

namespace EveSquadron.Models;

public static class AppConstants
{
    public const string ApplicationName = "Eve Squadron";
    public const string UserAgentHeader = $"{ApplicationName} - erythanadevsup@gmail.com";
    public static readonly string ConfigurationDirectory = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData), ApplicationName);

    #region Setting Defaults

    public const int MinimalClipboardPollingMs = 100;
    public const int MaximalClipboardPollingMs = 2000;
    public const int DefaultClipboardPollingMs = 250;
    public static readonly Color DefaultHoverColor = Colors.Orange;
    public const GridFontSizeEnum DefaultGridFontSize = GridFontSizeEnum.Medium;

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
