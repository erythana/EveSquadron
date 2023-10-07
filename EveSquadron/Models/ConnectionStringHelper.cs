using System.IO;
using Microsoft.Extensions.Configuration;

namespace EveSquadron.Models;

public static class ConnectionStringHelper
{
    public static string GetConnectionString(IConfiguration configuration)
    {
        var connectionString = configuration.GetConnectionString("EveSquadron");
        return string.IsNullOrWhiteSpace(connectionString)
            ? AppConstants.GetLocalConnectionString(Path.Combine(AppConstants.ConfigurationDirectory, AppConstants.SettingsDatabase))
            : connectionString;
    }
}