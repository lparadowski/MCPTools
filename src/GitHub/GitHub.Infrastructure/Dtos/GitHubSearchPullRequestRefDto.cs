using System.Text.Json.Serialization;

namespace GitHub.Infrastructure.Dtos;

internal class GitHubSearchPullRequestRefDto
{
    [JsonPropertyName("url")]
    public string? Url { get; set; }
}
