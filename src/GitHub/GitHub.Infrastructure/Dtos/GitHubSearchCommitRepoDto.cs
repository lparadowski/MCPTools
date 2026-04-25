using System.Text.Json.Serialization;

namespace GitHub.Infrastructure.Dtos;

internal class GitHubSearchCommitRepoDto
{
    [JsonPropertyName("full_name")]
    public string? FullName { get; set; }
}
