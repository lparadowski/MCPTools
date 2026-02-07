namespace Trello.Domain.Entities;

public class Comment
{
    public required string Id { get; set; }
    public string? Text { get; set; }
    public DateTime? Date { get; set; }
}
