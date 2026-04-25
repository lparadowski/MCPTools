namespace Jira.Api.Requests;

public class LinkIssuesRequest
{
    public required string InwardIssueKey { get; set; }
    public required string OutwardIssueKey { get; set; }
    public required string LinkTypeName { get; set; }
}
