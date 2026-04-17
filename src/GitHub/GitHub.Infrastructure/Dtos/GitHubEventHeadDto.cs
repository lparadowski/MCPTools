using System.Text.Json.Serialization;

namespace GitHub.Infrastructure.Dtos;

internal class GitHubEventHeadDto
{
    [JsonPropertyName("ref")]
    public string? Ref { get; set; }
}
