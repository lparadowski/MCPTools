namespace Jira.Domain.Entities;

public class AssignedIssue
{
    public required string Key { get; set; }
    public required string Summary { get; set; }
    public required string Status { get; set; }
}
