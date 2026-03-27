namespace Jira.Api.Responses;

public class AssignedIssueResponse
{
    public required string Key { get; set; }
    public required string Summary { get; set; }
    public required string Status { get; set; }
}
