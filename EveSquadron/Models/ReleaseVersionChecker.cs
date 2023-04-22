using System.Text.RegularExpressions;
using System.Threading.Tasks;
using EveSquadron.DataRepositories.Interfaces;
using EveSquadron.Models.Interfaces;
using Microsoft.Extensions.Logging;

namespace EveSquadron.Models;

public class GithubReleaseVersionChecker : IReleaseVersionChecker
{
    private readonly IGithubReleaseDataRepository _githubReleaseDataRepository;
    private readonly IReleaseSettingsLoader _releaseSettingsLoader;
    private readonly ILogger<GithubReleaseVersionChecker> _logger;

    public GithubReleaseVersionChecker(IGithubReleaseDataRepository githubReleaseDataRepository, IReleaseSettingsLoader releaseSettingsLoader, ILogger<GithubReleaseVersionChecker> logger)
    {
        _githubReleaseDataRepository = githubReleaseDataRepository;
        _releaseSettingsLoader = releaseSettingsLoader;
        _logger = logger;

        ReleasePath = _releaseSettingsLoader.ReleasePath;
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
        var releaseVersionEndpoint = _releaseSettingsLoader.ReleaseVersionAPIEndpoint;

        _logger.LogDebug($"Trying to get release version from '{releaseVersionEndpoint}'");
        var githubRelease = await _githubReleaseDataRepository.GetLatestReleaseInformationFrom(releaseVersionEndpoint);
        if (!TryParseVersionNumber(githubRelease.ReleaseName, out var serverVersion))
        {
            _logger.LogWarning($"Could not retrieve version information from '{releaseVersionEndpoint}'");
            return null;
        }

        //we know that its always going to be x.x.x - no need for iterating over
        var newReleaseAvailable = serverVersion.major > currentMajor ||
                                  serverVersion.major == currentMajor && serverVersion.minor > currentMinor ||
                                  serverVersion.major == currentMajor && serverVersion.minor == currentMinor && serverVersion.patch > currentPatch;
        
        if (newReleaseAvailable)
            _logger.LogInformation($"New release '{githubRelease}' available!");

        return newReleaseAvailable;
    }
}