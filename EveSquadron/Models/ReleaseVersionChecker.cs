using System.Text.RegularExpressions;
using System.Threading.Tasks;
using EveSquadron.DataRepositories.Interfaces;
using EveSquadron.Models.Interfaces;
using EveSquadron.Models.Options;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

namespace EveSquadron.Models;

public class GithubReleaseVersionChecker : IReleaseVersionChecker
{
    private readonly IGithubReleaseDataRepository _githubReleaseDataRepository;
    private readonly IOptions<ReleaseEndpointOptions> _releaseOptions;
    private readonly ILogger<GithubReleaseVersionChecker> _logger;

    public GithubReleaseVersionChecker(IGithubReleaseDataRepository githubReleaseDataRepository, IOptions<ReleaseEndpointOptions> releaseOptions, ILogger<GithubReleaseVersionChecker> logger)
    {
        _githubReleaseDataRepository = githubReleaseDataRepository;
        _releaseOptions = releaseOptions;
        _logger = logger;

        ReleasePath = _releaseOptions.Value.ReleasePath;
    }

    public string ReleasePath { get; }

    /// <summary>
    /// Tries to parse the version number. Returns (0.0.0) when it could not get parsed.
    /// </summary>
    /// <param name="version"></param>
    /// <param name="recognizedVersion"></param>
    /// <returns></returns>
    public bool TryParseVersionNumber(string version, out (int major, int minor, int patch) recognizedVersion)
    { 
        recognizedVersion = (0, 0, 0);
        
        var versionCheck = Regex.Match(version, "(\\d+)\\.(\\d+)\\.(\\d+)"); //Matches the three number groups
        if (versionCheck.Success)
            recognizedVersion = (int.Parse(versionCheck.Groups[1].Value), int.Parse(versionCheck.Groups[2].Value), int.Parse(versionCheck.Groups[3].Value));

        return versionCheck.Success;
    }

    /// <summary>
    /// Checks whether a new release is available.
    /// </summary>
    /// <param name="currentMajor"></param>
    /// <param name="currentMinor"></param>
    /// <param name="currentPatch"></param>
    /// <returns>Returns true when a new release is available, false when the current version is the latest, or null when there was a problem getting the servers version number.</returns>
    public async Task<bool?> IsNewReleaseAvailable(int currentMajor, int currentMinor, int currentPatch)
    {
        var releaseVersionEndpoint = _releaseOptions.Value.ReleaseVersionAPIEndpoint;

        _logger.LogDebug($"Trying to get release version from '{releaseVersionEndpoint}'");
        var githubRelease = await _githubReleaseDataRepository.GetLatestReleaseInformationFrom(releaseVersionEndpoint);
        if (!TryParseVersionNumber(githubRelease.ReleaseName, out var serverVersion))
        {
            _logger.LogWarning($"Could not retrieve version information from '{releaseVersionEndpoint}'");
            return null;
        }

        //we know that its always going to be x.x.x - no need for iterating over
        var newReleaseAvailable = serverVersion.major > currentMajor || serverVersion.minor > currentMinor || serverVersion.patch > currentPatch;
        
        if (newReleaseAvailable)
            _logger.LogInformation($"New release '{githubRelease}' available!");

        return newReleaseAvailable;
    }
}