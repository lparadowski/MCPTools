namespace Jira.Infrastructure.Dtos;

public class JiraSprintsResponseDto
{
    public int MaxResults { get; set; }
    public int StartAt { get; set; }
    public bool IsLast { get; set; }
    public List<JiraSprintDto>? Values { get; set; }
}
