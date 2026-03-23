namespace Jira.Infrastructure.Dtos;

public class JiraWorklogDto
{
    public string? Id { get; set; }
    public JiraUserDto? Author { get; set; }
    public string? TimeSpent { get; set; }
    public int? TimeSpentSeconds { get; set; }
    public JiraDocumentDto? Comment { get; set; }
    public string? Started { get; set; }
    public string? Created { get; set; }
    public string? Updated { get; set; }
}
