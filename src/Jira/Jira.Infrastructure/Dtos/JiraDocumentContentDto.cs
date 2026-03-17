namespace Jira.Infrastructure.Dtos;

public class JiraDocumentContentDto
{
    public string? Type { get; set; }
    public List<JiraDocumentTextDto>? Content { get; set; }
}
