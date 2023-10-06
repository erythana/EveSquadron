using System;
using System.Globalization;
using System.IO;
using System.Threading.Tasks;
using CsvHelper;
using EveSquadron.Models.CsvHelper;
using EveSquadron.Models.EveSquadron;

namespace EveSquadron.Models;

public class EveSquadronCsvExport : IEveSquadronCsvExport
{
    public async Task ExportToCsv(string autoExportFile, EveSquadronPlayerInformation playerInformation, DateTime lastScanned)
    {
        if (string.IsNullOrWhiteSpace(autoExportFile))
            return;

        var appendHeader = !File.Exists(autoExportFile);

        await using var writer = new StreamWriter(autoExportFile, new FileStreamOptions
        {
            Mode = FileMode.Append,
            Access = FileAccess.Write
        });
        await using var csv = new CsvWriter(writer, CultureInfo.InvariantCulture);
        csv.Context.RegisterClassMap(new EveSquadronInformationCsvMap());
        if (appendHeader)
        {
            csv.WriteHeader<EveSquadronPlayerInformation>();
            csv.WriteField("Last scanned");
        }

        await csv.NextRecordAsync();
        csv.WriteRecord(playerInformation);
        csv.WriteField(lastScanned.ToString(CultureInfo.CurrentCulture));
    }

}

public interface IEveSquadronCsvExport
{
    public Task ExportToCsv(string path, EveSquadronPlayerInformation playerInformation, DateTime lastScanned);
}