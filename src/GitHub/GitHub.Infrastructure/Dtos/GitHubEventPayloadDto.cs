using System.Text.Json.Serialization;

namespace GitHub.Infrastructure.Dtos;

internal class GitHubEventPayloadDto
{
    [JsonPropertyName("action")]
    public string? Action { get; set; }

    [JsonPropertyName("ref")]
    public string? Ref { get; set; }

    [JsonPropertyName("ref_type")]
    public string? RefType { get; set; }

    [JsonPropertyName("number")]
    public int? Number { get; set; }

    [JsonPropertyName("pull_request")]
    public GitHubEventPullRequestDto? PullRequest { get; set; }

    [JsonPropertyName("issue")]
    public GitHubEventIssueDto? Issue { get; set; }

    [JsonPropertyName("comment")]
    public GitHubEventCommentDto? Comment { get; set; }

    [JsonPropertyName("head")]
    public string? Head { get; set; }

    [JsonPropertyName("size")]
    public int? Size { get; set; }

    [JsonPropertyName("commits")]
    public List<GitHubEventCommitDto>? Commits { get; set; }

    [JsonPropertyName("review")]
    public GitHubEventReviewDto? Review { get; set; }
}
