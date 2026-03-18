namespace Jira.Domain.Entities;

public class Transition
{
    public required string Id { get; set; }
    public required string Name { get; set; }
    public string? ToStatus { get; set; }
}
