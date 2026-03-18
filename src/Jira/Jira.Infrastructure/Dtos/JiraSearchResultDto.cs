namespace Jira.Infrastructure.Dtos;

public class JiraSearchResultDto
{
    public int StartAt { get; set; }
    public int MaxResults { get; set; }
    public int Total { get; set; }
    public List<JiraIssueDto>? Issues { get; set; }
}
