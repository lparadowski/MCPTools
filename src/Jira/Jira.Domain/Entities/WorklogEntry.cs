namespace Jira.Domain.Entities;

public class WorklogEntry
{
    public required string Author { get; set; }
    public double Hours { get; set; }
    public string? Comment { get; set; }
    public DateTime Started { get; set; }
}
