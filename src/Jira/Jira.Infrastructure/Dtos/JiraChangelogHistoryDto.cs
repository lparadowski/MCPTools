namespace Jira.Infrastructure.Dtos;

public class JiraChangelogHistoryDto
{
    public string? Id { get; set; }
    public JiraUserDto? Author { get; set; }
    public string? Created { get; set; }
    public List<JiraChangelogItemDto>? Items { get; set; }
}
