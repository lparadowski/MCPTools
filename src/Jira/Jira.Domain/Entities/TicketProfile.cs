namespace Jira.Domain.Entities;

public class TicketProfile
{
    public required string Key { get; set; }
    public required string Summary { get; set; }
    public required string IssueType { get; set; }
    public required string Status { get; set; }
    public string? Assignee { get; set; }
    public DateTime Created { get; set; }
    public DateTime Updated { get; set; }
    public Dictionary<string, double> StatusDurationsHours { get; set; } = new();
    public List<StatusTransition> StatusTransitions { get; set; } = [];
    public double TotalWorklogHours { get; set; }
    public List<WorklogEntry> Worklogs { get; set; } = [];
}
