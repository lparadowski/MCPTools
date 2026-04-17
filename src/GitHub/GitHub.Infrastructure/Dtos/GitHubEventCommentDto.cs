using System.Text.Json.Serialization;

namespace GitHub.Infrastructure.Dtos;

internal class GitHubEventCommentDto
{
    [JsonPropertyName("body")]
    public string? Body { get; set; }
}
