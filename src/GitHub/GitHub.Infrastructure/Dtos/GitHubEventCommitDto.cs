using System.Text.Json.Serialization;

namespace GitHub.Infrastructure.Dtos;

internal class GitHubEventCommitDto
{
    [JsonPropertyName("sha")]
    public string? Sha { get; set; }

    [JsonPropertyName("message")]
    public string? Message { get; set; }
}
