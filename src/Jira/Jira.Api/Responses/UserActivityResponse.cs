namespace Jira.Api.Responses;

public class UserActivityResponse
{
    public required DateTime Date { get; set; }
    public List<AssignedIssueResponse> AssignedIssues { get; set; } = [];
    public List<ChangelogEntryResponse> Changes { get; set; } = [];
    public List<ActivityCommentResponse> Comments { get; set; } = [];
}
