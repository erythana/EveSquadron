namespace EveSquadron.DataAccess;

public partial class SqLiteConfigurationProvider
{
    private static partial class SqLiteStatements
    {
        public static string ApplicationSettings = "SELECT Name, Value From ApplicationSettings";
    }
}