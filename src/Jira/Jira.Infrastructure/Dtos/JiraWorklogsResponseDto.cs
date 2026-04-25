namespace Jira.Infrastructure.Dtos;

public class JiraWorklogsResponseDto
{
    public List<JiraWorklogDto>? Worklogs { get; set; }
    public int? Total { get; set; }
}
