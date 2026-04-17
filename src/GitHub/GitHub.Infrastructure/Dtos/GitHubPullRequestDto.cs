using System.Text.Json.Serialization;

namespace GitHub.Infrastructure.Dtos;

internal class GitHubPullRequestDto
{
    [JsonPropertyName("number")]
    public int Number { get; set; }

    [JsonPropertyName("title")]
    public string Title { get; set; } = string.Empty;

    [JsonPropertyName("state")]
    public string State { get; set; } = string.Empty;

    [JsonPropertyName("user")]
    public GitHubUserDto? User { get; set; }

    [JsonPropertyName("body")]
    public string? Body { get; set; }

    [JsonPropertyName("head")]
    public GitHubPullRequestBranchDto? Head { get; set; }

    [JsonPropertyName("base")]
    public GitHubPullRequestBranchDto? Base { get; set; }

    [JsonPropertyName("draft")]
    public bool Draft { get; set; }

    [JsonPropertyName("html_url")]
    public string? HtmlUrl { get; set; }

    [JsonPropertyName("created_at")]
    public DateTime? CreatedAt { get; set; }

    [JsonPropertyName("updated_at")]
    public DateTime? UpdatedAt { get; set; }

    [JsonPropertyName("merged_at")]
    public DateTime? MergedAt { get; set; }
}
