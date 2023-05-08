using EveSquadronKillmailImporter.Interfaces;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;

namespace EveSquadronKillmailImporter;

/*
 * Set up the Database-Connection String. This tool expects to find a table called "Killmail" which has the attributes of the Model\Killmail class.
 * Set the Mode to either "File" (for import from "https://data.everef.net/killmails/") or "ZkillboardRedis" to import killmails as ZKillboard receives them.
 * On "File"-Mode it expects the tar.bz2 files in the configured "KillmailDirectory" path, this needs to be an absolute path. It extracts and imports all files it can find.
 * On "ZkillboardRedis"-Mode it imports all killmails it receives from the endpoint.
 *      You can configure to either shutdown if there are no new killmails (default, to save costs on Azure - reschedule execution within the Redis Timelimit)
 *      OR you can set "KeepListening" to true.
 */

internal class Program
    {
        static void Main(string[] args)
        {
            var configuration = new ConfigurationBuilder()
                .SetBasePath(Directory.GetCurrentDirectory())
                .AddJsonFile("appsettings.json")
                .Build();
            
            var eveSquadronDataAccess = new EveSquadronPostgreSqlDataAccess(configuration);
            var dataRepository = new EveSquadronDataRepository(eveSquadronDataAccess);
            
            IProcessKillmails selectedSource = configuration.GetSection("Mode").Value switch
            {
                "File" => new FileSourceImporter(configuration, dataRepository),
                "ZKillboardRedis" => new ZKillboardSourceImporter(configuration, dataRepository),
                _ => throw new ArgumentException("The selected mode does not exist")
            };
            
            selectedSource.ProcessKillmails().GetAwaiter().GetResult();
        }
    }