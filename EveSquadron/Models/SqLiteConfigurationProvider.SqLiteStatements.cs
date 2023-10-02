namespace EveSquadron.Models;

public partial class SqLiteConfigurationProvider
{
    private static partial class SqLiteStatements
    {
        public static string ApplicationSettings = "SELECT * From ApplicationSettings";
    }
}