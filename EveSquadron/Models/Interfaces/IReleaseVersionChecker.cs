using System.Threading.Tasks;

namespace EveSquadron.Models.Interfaces;

public interface IReleaseVersionChecker
{
    public string ReleasePath { get; }
    public bool TryParseVersionNumber(string version, out (int major, int minor, int patch) recognizedVersion);
    public Task<bool?> IsNewReleaseAvailable(int currentMajor, int currentMinor, int currentPatch);
}