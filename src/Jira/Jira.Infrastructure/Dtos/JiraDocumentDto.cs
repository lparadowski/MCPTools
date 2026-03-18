namespace Jira.Infrastructure.Dtos;

public class JiraDocumentDto
{
    public string? Type { get; set; }
    public int? Version { get; set; }
    public List<JiraDocumentContentDto>? Content { get; set; }
}
