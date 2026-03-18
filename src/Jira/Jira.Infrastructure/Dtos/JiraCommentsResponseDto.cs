namespace Jira.Infrastructure.Dtos;

public class JiraCommentsResponseDto
{
    public int StartAt { get; set; }
    public int MaxResults { get; set; }
    public int Total { get; set; }
    public List<JiraCommentDto>? Comments { get; set; }
}
