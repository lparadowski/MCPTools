namespace Jira.Api.Responses;

public class ActivityCommentResponse
{
    public required string IssueKey { get; set; }
    public required string IssueSummary { get; set; }
    public string? Body { get; set; }
    public required DateTime Created { get; set; }
}
