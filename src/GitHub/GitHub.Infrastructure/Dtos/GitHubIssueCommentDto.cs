using System.Text.Json.Serialization;

namespace GitHub.Infrastructure.Dtos;

internal class GitHubIssueCommentDto
{
    [JsonPropertyName("id")]
    public long Id { get; set; }

    [JsonPropertyName("user")]
    public GitHubUserDto? User { get; set; }

    [JsonPropertyName("body")]
    public string Body { get; set; } = string.Empty;

    [JsonPropertyName("created_at")]
    public DateTime CreatedAt { get; set; }

    [JsonPropertyName("updated_at")]
    public DateTime? UpdatedAt { get; set; }
}
