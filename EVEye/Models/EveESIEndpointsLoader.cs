using System;
using EVEye.Models.Interfaces;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;

namespace EVEye.Models
{
    public class EveESIEndpointsLoader : IEveESIEndpointsLoader
    {
        #region member fields

        
        private readonly IConfiguration _configuration;
        private readonly ILogger<EveESIEndpointsLoader> _logger;
        private readonly string _esiEndpointPath;

        #endregion

        #region constructor

        
        public EveESIEndpointsLoader(IConfiguration configuration, ILogger<EveESIEndpointsLoader> logger)
        {
            _configuration = configuration;
            _esiEndpointPath = "Endpoints:EveESI";
            
            _logger = logger;
            UniverseEndpoint = LoadSetting("UniverseEndpoint");
            PortraitEndpoint = LoadSetting("PortraitEndpoint");
        }

        #endregion

        #region properties

        public string UniverseEndpoint { get; }
        public string PortraitEndpoint { get; }

        #endregion

        #region helper methods

        private string LoadSetting(string settingName, bool required = true)
        {
            var value = _configuration.GetValue<string>($"{_esiEndpointPath}:{settingName}");
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