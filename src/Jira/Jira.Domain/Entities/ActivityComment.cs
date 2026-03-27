namespace Jira.Domain.Entities;

public class ActivityComment
{
    public required string IssueKey { get; set; }
    public required string IssueSummary { get; set; }
    public string? Body { get; set; }
    public required DateTime Created { get; set; }
}
