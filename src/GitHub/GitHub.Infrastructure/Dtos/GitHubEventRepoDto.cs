using System.Text.Json.Serialization;

namespace GitHub.Infrastructure.Dtos;

internal class GitHubEventRepoDto
{
    [JsonPropertyName("name")]
    public string Name { get; set; } = string.Empty;
}
