namespace Jira.Domain.Entities;

public class IssueLink
{
    public required string Id { get; set; }
    public required string TypeName { get; set; }
    public string? InwardIssueKey { get; set; }
    public string? OutwardIssueKey { get; set; }
}
