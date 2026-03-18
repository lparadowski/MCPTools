namespace Jira.Infrastructure.Dtos;

public class JiraIssueLinkDto
{
    public string? Id { get; set; }
    public JiraIssueLinkTypeDto? Type { get; set; }
    public JiraLinkedIssueDto? InwardIssue { get; set; }
    public JiraLinkedIssueDto? OutwardIssue { get; set; }
}
