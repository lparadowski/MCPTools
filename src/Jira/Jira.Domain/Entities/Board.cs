namespace Jira.Domain.Entities;

public class Board
{
    public required int Id { get; set; }
    public required string Name { get; set; }
    public string? Type { get; set; }
    public string? ProjectKey { get; set; }
}
