using System.Text.Json.Serialization;

namespace GitHub.Infrastructure.Dtos;

internal class GitHubReviewDto
{
    [JsonPropertyName("id")]
    public long Id { get; set; }

    [JsonPropertyName("user")]
    public GitHubUserDto? User { get; set; }

    [JsonPropertyName("state")]
    public string State { get; set; } = string.Empty;

    [JsonPropertyName("body")]
    public string? Body { get; set; }

    [JsonPropertyName("submitted_at")]
    public DateTime SubmittedAt { get; set; }
}
