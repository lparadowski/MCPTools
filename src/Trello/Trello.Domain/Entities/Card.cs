namespace Trello.Domain.Entities;

public class Card
{
    public required string Id { get; set; }
    public required string Name { get; set; }
    public string? Description { get; set; }
    public string? ListName { get; set; }
    public string? ListId { get; set; }
    public string? Url { get; set; }
    public DateTime? LastActivity { get; set; }
    public List<Label> Labels { get; set; } = [];
    public List<Comment> Comments { get; set; } = [];
}
