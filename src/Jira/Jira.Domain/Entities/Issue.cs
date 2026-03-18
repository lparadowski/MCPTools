namespace Jira.Domain.Entities;

public class Issue
{
    public required string Id { get; set; }
    public required string Key { get; set; }
    public required string Summary { get; set; }
    public string? Description { get; set; }
    public required string IssueType { get; set; }
    public required string Status { get; set; }
    public string? Priority { get; set; }
    public string? AssigneeAccountId { get; set; }
    public string? AssigneeDisplayName { get; set; }
    public string? ReporterDisplayName { get; set; }
    public string? ProjectKey { get; set; }
    public string? ParentKey { get; set; }
    public List<string> Labels { get; set; } = [];
    public DateTime? Created { get; set; }
    public DateTime? Updated { get; set; }
}
