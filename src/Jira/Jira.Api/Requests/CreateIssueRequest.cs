namespace Jira.Api.Requests;

public class CreateIssueRequest
{
    public required string ProjectKey { get; set; }
    public required string IssueType { get; set; }
    public required string Summary { get; set; }
    public string? Description { get; set; }
    public string? ParentKey { get; set; }
    public List<string>? Labels { get; set; }
}
