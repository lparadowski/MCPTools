using System.Text.Json.Serialization;

namespace GitHub.Infrastructure.Dtos;

internal class GitHubSearchCommitItemDto
{
    [JsonPropertyName("sha")]
    public string Sha { get; set; } = string.Empty;

    [JsonPropertyName("commit")]
    public GitHubSearchCommitDetailDto? Commit { get; set; }

    [JsonPropertyName("repository")]
    public GitHubSearchCommitRepoDto? Repository { get; set; }

    [JsonPropertyName("html_url")]
    public string? HtmlUrl { get; set; }
}
