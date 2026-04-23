using System.Text.Json.Serialization;

namespace GitHub.Infrastructure.Dtos;

internal class GitHubSearchCommitDetailDto
{
    [JsonPropertyName("message")]
    public string? Message { get; set; }

    [JsonPropertyName("author")]
    public GitHubSearchCommitAuthorDto? Author { get; set; }
}
