namespace Jira.Api.Responses;

public class TicketProfileResponse
{
    public required string Key { get; set; }
    public required string Summary { get; set; }
    public required string IssueType { get; set; }
    public required string Status { get; set; }
    public string? Assignee { get; set; }
    public DateTime Created { get; set; }
    public DateTime Updated { get; set; }
    public Dictionary<string, double> StatusDurationsHours { get; set; } = new();
    public List<StatusTransitionResponse> StatusTransitions { get; set; } = [];
    public double TotalWorklogHours { get; set; }
    public List<WorklogEntryResponse> Worklogs { get; set; } = [];
}
