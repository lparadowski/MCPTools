namespace Jira.Domain.Entities;

public class UserActivity
{
    public required DateTime Date { get; set; }
    public List<AssignedIssue> AssignedIssues { get; set; } = [];
    public List<ChangelogEntry> Changes { get; set; } = [];
    public List<ActivityComment> Comments { get; set; } = [];
}
