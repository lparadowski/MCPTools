namespace GitHub.Domain.Entities;

public class Review
{
    public required long Id { get; set; }
    public required string Author { get; set; }
    public required string State { get; set; }
    public string? Body { get; set; }
    public required DateTime SubmittedAt { get; set; }
}
