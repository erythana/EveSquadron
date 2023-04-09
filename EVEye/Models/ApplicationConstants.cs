using System.Text.Json;
using Newtonsoft.Json;

namespace EVEye.Models
{
    public static class ApplicationConstants
    {
        public const string ApplicationName = "EVEye";
        public const string UserAgentHeader = $"erythanadevsup@gmail.com";

        public static class EveAPILimits
        {
            public const int PostUniverseIDsCharactersLimit = 500;
            public const int PostUniverseNamesIDsLimit = 1000;
        }
        
        public static readonly JsonSerializerOptions AppDefaultSerializerOptions = new()
        {
            WriteIndented = true,
            PropertyNameCaseInsensitive = true
        };
    }
}