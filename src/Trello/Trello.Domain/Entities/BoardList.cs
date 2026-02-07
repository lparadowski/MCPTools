namespace Trello.Domain.Entities;

public class BoardList
{
    public required string Id { get; set; }
    public required string Name { get; set; }
    public List<Card> Cards { get; set; } = [];
}
