namespace Jira.Api.Responses;

public class CommentResponse
{
    public required string Id { get; set; }
    public string? AuthorDisplayName { get; set; }
    public string? Body { get; set; }
    public DateTime? Created { get; set; }
    public DateTime? Updated { get; set; }
}
