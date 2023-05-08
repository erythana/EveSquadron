using System.Text.Json;
using EveSquadronKillmailImporter.Interfaces;
using EveSquadronKillmailImporter.Models;
using Microsoft.Extensions.Configuration;
using SharpCompress.Common;
using SharpCompress.Readers;

namespace EveSquadronKillmailImporter;

internal class FileSourceImporter : IProcessKillmails
{
    #region member fields

    private readonly IEveSquadronDataRepository _eveSquadronDataRepository;
    private readonly string _killmailInputDirectory;

    #endregion

    #region constructor

    public FileSourceImporter(IConfiguration configuration, IEveSquadronDataRepository eveSquadronDataRepository)
    {
        _eveSquadronDataRepository = eveSquadronDataRepository;
        _killmailInputDirectory = configuration.GetSection("FileModeSettings:KillmailDirectory").Value!;
        if (string.IsNullOrWhiteSpace(_killmailInputDirectory) || !Directory.Exists(_killmailInputDirectory))
            throw new InvalidOperationException("Did not find 'FileModeSettings:KillmailDirectory' setting or the directory does not exist.");
    }

    #endregion

    #region interface implementation
    
    public async Task ProcessKillmails()
    {
        foreach (var file in Directory.GetFiles(_killmailInputDirectory))
        {
            var tempExtractDirectory = Directory.CreateTempSubdirectory("EveSquadronTempDirectory-").FullName;
            ExtractArchive(file, tempExtractDirectory);

            var killmails = ProcessKillmailsInDirectory(tempExtractDirectory);
            try
            {
                var eveSquadronKillmails = killmails.Select(x => new Killmail
                {
                    ID = x.killmail_id,
                    KillmailHash = x.killmail_hash,
                    SolarSystemID = x.solar_system_id,
                    WinCharacterID = x.attackers.First(x => x.final_blow).character_id,
                    WinShipTypeID = x.attackers.First(x => x.final_blow).ship_type_id,
                    WinWeaponTypeID = x.attackers.First(x => x.final_blow).weapon_type_id,
                    WinAttackerCount = x.attackers.Count,
                    LossCharacterID = x.victim.character_id,
                    LossShipTypeID = x.victim.ship_type_id,
                    KillmailDate = x.killmail_time
                });

                var squadronKillmails = eveSquadronKillmails.ToList();
                var alreadySavedIDs = await _eveSquadronDataRepository.GetAlreadySavedKillmails(squadronKillmails);
                var missingKillmails = squadronKillmails.Where(x => !alreadySavedIDs.Contains(x.ID));

                await _eveSquadronDataRepository.SaveKillmails(missingKillmails);
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
                throw;
            }
            finally
            {
                CleanupTempDirectory(tempExtractDirectory);
            }
        }
    }
    
    #endregion

    #region helper methods

    private IEnumerable<EveKillmail> ProcessKillmailsInDirectory(string killmailDirectory)
    {
        var allKills = new List<EveKillmail>();
        var killmailFiles = Directory.GetFiles(killmailDirectory, "*.json", SearchOption.AllDirectories);
        foreach (var killmailFile in killmailFiles)
        {
            var content = File.ReadAllText(killmailFile);
            allKills.Add(JsonSerializer.Deserialize<EveKillmail>(content)!);
        }

        return allKills;
    }
    
    private static void ExtractArchive(string file, string targetDirectory)
    {

        using Stream stream = File.OpenRead(file);
        using var reader = ReaderFactory.Open(stream);
        while (reader.MoveToNextEntry())
        {
            if (reader.Entry.IsDirectory)
                continue;
            reader.WriteEntryToDirectory(targetDirectory, new ExtractionOptions()
            {
                ExtractFullPath = true,
                Overwrite = true
            });
        }

    }

    private static void CleanupTempDirectory(string tempDirectory)
    {
        var di = new DirectoryInfo(tempDirectory);
        di.Delete(true);
    }

    #endregion
}