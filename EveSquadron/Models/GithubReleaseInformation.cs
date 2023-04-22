using System.Text.Json.Serialization;

namespace EveSquadron.Models;

public class GithubReleaseInformation
{
    [JsonPropertyName("Name")]
    public string ReleaseName { get; set; }
}