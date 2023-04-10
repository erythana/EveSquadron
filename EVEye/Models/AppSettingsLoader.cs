using System;
using EVEye.Models.Interfaces;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;

namespace EVEye.Models
{
    public class AppSettingsLoader : IAppSettingsLoader
    {
        #region member fields

        private readonly IConfiguration _configuration;
        private readonly ILogger<AppSettingsLoader> _logger;

        #endregion
        #region constructor

        public AppSettingsLoader(IConfiguration configuration, ILogger<AppSettingsLoader> logger)
        {
            _configuration = configuration;

            _logger = logger;
            ClipboardPollingMilliseconds = LoadSetting("ClipboardPollingMilliseconds", false);
            Theme = LoadSetting("Theme", false);
        }

        #endregion

        #region properties

        public string ClipboardPollingMilliseconds { get; set; }
        
        public string Theme { get; set; }

        #endregion

        #region helper methods

        private string LoadSetting(string settingName, bool required = true)
        {
            var value = _configuration.GetValue<string>($"{settingName}");
            if (!required || !string.IsNullOrWhiteSpace(value))
                return value ?? string.Empty;
            
            var error = $"Can not load required setting '{settingName}'";
            _logger.LogCritical(error);
            throw new InvalidOperationException(error);
        }

        #endregion
    }
}