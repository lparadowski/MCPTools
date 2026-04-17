using System.Text.Json.Serialization;

namespace GitHub.Infrastructure.Dtos;

internal class GitHubPullRequestBranchDto
{
    [JsonPropertyName("ref")]
    public string? Ref { get; set; }
}
