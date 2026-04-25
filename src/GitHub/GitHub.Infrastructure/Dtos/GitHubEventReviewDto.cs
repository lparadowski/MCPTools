using System.Text.Json.Serialization;

namespace GitHub.Infrastructure.Dtos;

internal class GitHubEventReviewDto
{
    [JsonPropertyName("state")]
    public string? State { get; set; }

    [JsonPropertyName("body")]
    public string? Body { get; set; }
}
