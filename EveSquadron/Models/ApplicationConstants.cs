using System.Text.Json;

namespace EveSquadron.Models;

public static class ApplicationConstants
{
    public const string ApplicationName = "Eve Squadron";
    public const string UserAgentHeader = "erythanadevsup@gmail.com";

    public static readonly JsonSerializerOptions AppDefaultSerializerOptions = new()
    {
        WriteIndented = true,
        PropertyNameCaseInsensitive = true
    };

    public static class ZKillboardAPILimits
    {
        public const int ZKillboardRateLimitMs = 200;
    }

    public static class EveAPILimits
    {
        public const int PostUniverseIDsCharactersLimit = 500;
        public const int PostUniverseNamesIDsLimit = 1000;
    }
}