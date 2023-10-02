namespace EveSquadron.Models;

public partial class SqLiteConfigurationSource
{
    private static partial class SqLiteStatements
    {
        public static string CreateApplicationSettingsTable = @"
        CREATE TABLE IF NOT EXISTS [ApplicationSettings] (
            [Name] text NOT NULL,
            [Value] text NOT NULL
        )";

        public const string CreateWhitelistEntriesTable = @"
        CREATE TABLE IF NOT EXISTS [WhitelistEntries] (
            [Type] INTEGER NOT NULL,
            [Name] text NOT NULL
        )";
    }
}