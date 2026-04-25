using System.Text.Json.Serialization;

namespace GitHub.Infrastructure.Dtos;

internal class GitHubSearchIssueItemDto
{
    [JsonPropertyName("id")]
    public long Id { get; set; }

    [JsonPropertyName("number")]
    public int Number { get; set; }

    [JsonPropertyName("title")]
    public string Title { get; set; } = string.Empty;

    [JsonPropertyName("state")]
    public string? State { get; set; }

    [JsonPropertyName("body")]
    public string? Body { get; set; }

    [JsonPropertyName("repository_url")]
    public string? RepositoryUrl { get; set; }

    [JsonPropertyName("html_url")]
    public string? HtmlUrl { get; set; }

    [JsonPropertyName("created_at")]
    public DateTime CreatedAt { get; set; }

    [JsonPropertyName("updated_at")]
    public DateTime? UpdatedAt { get; set; }

    [JsonPropertyName("pull_request")]
    public GitHubSearchPullRequestRefDto? PullRequest { get; set; }
}
