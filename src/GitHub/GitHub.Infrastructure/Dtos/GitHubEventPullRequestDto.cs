using System.Text.Json.Serialization;

namespace GitHub.Infrastructure.Dtos;

internal class GitHubEventPullRequestDto
{
    [JsonPropertyName("title")]
    public string? Title { get; set; }

    [JsonPropertyName("number")]
    public int? Number { get; set; }

    [JsonPropertyName("head")]
    public GitHubEventHeadDto? Head { get; set; }
}
