namespace Jira.Infrastructure.Dtos;

public class JiraBoardsResponseDto
{
    public int MaxResults { get; set; }
    public int StartAt { get; set; }
    public int Total { get; set; }
    public bool IsLast { get; set; }
    public List<JiraBoardDto>? Values { get; set; }
}
