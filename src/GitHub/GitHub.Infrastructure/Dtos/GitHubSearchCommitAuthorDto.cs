using System.Text.Json.Serialization;

namespace GitHub.Infrastructure.Dtos;

internal class GitHubSearchCommitAuthorDto
{
    [JsonPropertyName("date")]
    public DateTime? Date { get; set; }
}
