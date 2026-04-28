namespace GitHub.Domain.Entities;

public class ReviewComment
{
    public required long Id { get; set; }
    public required string Author { get; set; }
    public required string Body { get; set; }
    public required string Path { get; set; }
    public int? Line { get; set; }
    public string? Side { get; set; }
    public required DateTime CreatedAt { get; set; }
    public DateTime? UpdatedAt { get; set; }
}
