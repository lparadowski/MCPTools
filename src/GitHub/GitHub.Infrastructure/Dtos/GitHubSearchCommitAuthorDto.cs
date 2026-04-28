using System.Text.Json.Serialization;

namespace GitHub.Infrastructure.Dtos;

internal class GitHubSearchCommitAuthorDto
{
    [JsonPropertyName("name")]
    public string? Name { get; set; }

    [JsonPropertyName("date")]
    public DateTime? Date { get; set; }
}
