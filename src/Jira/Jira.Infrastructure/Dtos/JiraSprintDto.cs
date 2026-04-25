namespace Jira.Infrastructure.Dtos;

public class JiraSprintDto
{
    public int Id { get; set; }
    public string? Name { get; set; }
    public string? State { get; set; }
    public string? Goal { get; set; }
    public string? StartDate { get; set; }
    public string? EndDate { get; set; }
}
