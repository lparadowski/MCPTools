namespace Jira.Infrastructure.Dtos;

public class JiraChangelogResponseDto
{
    public int StartAt { get; set; }
    public int MaxResults { get; set; }
    public int Total { get; set; }
    public List<JiraChangelogHistoryDto>? Values { get; set; }
}
