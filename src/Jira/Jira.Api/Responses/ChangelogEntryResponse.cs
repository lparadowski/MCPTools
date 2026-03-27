namespace Jira.Api.Responses;

public class ChangelogEntryResponse
{
    public required string IssueKey { get; set; }
    public required string IssueSummary { get; set; }
    public required string Field { get; set; }
    public string? From { get; set; }
    public string? To { get; set; }
    public required DateTime Changed { get; set; }
}
