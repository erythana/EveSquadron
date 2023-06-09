using System.Text.Json;

namespace EveSquadron.Models;

public static class AppConstants
{
    public const string ApplicationName = "Eve Squadron";
    public const string UserAgentHeader = $"{ApplicationName} - erythanadevsup@gmail.com";

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
}
