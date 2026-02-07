namespace Trello.Api.Responses;

public class CommentResponse
{
    public required string Id { get; set; }
    public string? Text { get; set; }
    public DateTime? Date { get; set; }
}
