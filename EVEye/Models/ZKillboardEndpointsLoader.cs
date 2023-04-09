using System;
using EVEye.Models.Interfaces;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;

namespace EVEye.Models
{
    public class ZKillboardEndpointsLoader : IZKillboardEndpointsLoader
    {
        #region member fields
        
        private readonly IConfiguration _configuration;
        private readonly ILogger<ZKillboardEndpointsLoader> _logger;
        private readonly string _zKillboardEndpointPath;

        #endregion

        #region constructor
        
        public ZKillboardEndpointsLoader(IConfiguration configuration, ILogger<ZKillboardEndpointsLoader> logger)
        {
            _configuration = configuration;
            _logger = logger;

            _zKillboardEndpointPath = "Endpoints:ZKillboard";

            CharacterStatsEndpoint = LoadSetting("CharacterStatsEndpoint");
        }
        
        #endregion

        #region properties

        public string CharacterStatsEndpoint { get; }

        #endregion

        #region helper methods

        private string LoadSetting(string settingName, bool required = true)
        {
            var value = _configuration.GetValue<string>($"{_zKillboardEndpointPath}:{settingName}");
            if (required && string.IsNullOrWhiteSpace(value))
            {
                var error = $"The EVE-ESI endpoint for '{settingName}' is not set";
                _logger.LogCritical(error);
                throw new InvalidOperationException(error);
            }

            return value ?? string.Empty;
        }

        #endregion
    }

}