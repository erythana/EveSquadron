namespace EveSquadron.DataAccess;

public partial class ApplicationSettingsDataAccess
{
    private partial class SqLiteStatements
    {
        public static string InsertReplaceApplicationSettings = "REPLACE INTO ApplicationSettings (Name, Value) VALUES (@Name, @Value)";
    }
}