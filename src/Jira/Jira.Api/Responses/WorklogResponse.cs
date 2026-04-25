namespace Jira.Api.Responses;

public class WorklogResponse
{
    public required string Id { get; set; }
    public string? IssueKey { get; set; }
    public string? AuthorDisplayName { get; set; }
    public string? TimeSpent { get; set; }
    public int? TimeSpentSeconds { get; set; }
    public string? Comment { get; set; }
    public DateTime? Started { get; set; }
    public DateTime? Created { get; set; }
    public DateTime? Updated { get; set; }
}
