namespace Jira.Infrastructure.Dtos;

public class JiraIssueDto
{
    public string? Id { get; set; }
    public string? Key { get; set; }
    public string? Self { get; set; }
    public Dictionary<string, string>? Names { get; set; }
    public JiraIssueFieldsDto? Fields { get; set; }
}
