namespace Jira.Api.Requests;

public class AddWorklogRequest
{
    public required string TimeSpent { get; set; }
    public string? Comment { get; set; }
    public DateTime? Started { get; set; }
}
