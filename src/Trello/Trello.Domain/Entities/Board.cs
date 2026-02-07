namespace Trello.Domain.Entities;

public class Board
{
    public required string Id { get; set; }
    public required string Name { get; set; }
    public string? Description { get; set; }
    public string? Url { get; set; }
    public List<BoardList> Lists { get; set; } = [];
}
