namespace Trello.Api.Responses;

public class CardResponse
{
    public required string Id { get; set; }
    public required string Name { get; set; }
    public string? Description { get; set; }
    public string? ListName { get; set; }
    public string? Url { get; set; }
    public DateTime? LastActivity { get; set; }
    public List<LabelResponse> Labels { get; set; } = [];
    public List<CommentResponse> Comments { get; set; } = [];
}
