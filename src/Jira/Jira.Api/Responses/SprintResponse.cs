namespace Jira.Api.Responses;

public class SprintResponse
{
    public int Id { get; set; }
    public required string Name { get; set; }
    public string? State { get; set; }
    public string? Goal { get; set; }
    public DateTime? StartDate { get; set; }
    public DateTime? EndDate { get; set; }
}
