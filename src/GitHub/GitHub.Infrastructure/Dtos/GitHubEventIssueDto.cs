using System.Text.Json.Serialization;

namespace GitHub.Infrastructure.Dtos;

internal class GitHubEventIssueDto
{
    [JsonPropertyName("title")]
    public string? Title { get; set; }

    [JsonPropertyName("number")]
    public int? Number { get; set; }
}
