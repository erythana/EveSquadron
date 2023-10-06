using CsvHelper.Configuration;
using EveSquadron.Models.EveSquadron;

namespace EveSquadron.Models.CsvHelper;

public sealed class EveSquadronInformationCsvMap : ClassMap<EveSquadronPlayerInformation>
{
    public EveSquadronInformationCsvMap()
    {
        Map(m => m.ID).Name("ID");
        Map(m => m.Character.Name).Name("Name");
        Map(m => m.Alliance!.Name).Name("Alliance");
        Map(m => m.SecurityStanding).Name("Security status");
    }
}