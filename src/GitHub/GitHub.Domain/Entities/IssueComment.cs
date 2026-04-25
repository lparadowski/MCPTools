namespace GitHub.Domain.Entities;

public class IssueComment
{
    public required long Id { get; set; }
    public required string Author { get; set; }
    public required string Body { get; set; }
    public required DateTime CreatedAt { get; set; }
    public DateTime? UpdatedAt { get; set; }
}
