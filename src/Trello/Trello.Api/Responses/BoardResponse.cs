namespace Trello.Api.Responses;

public class BoardResponse
{
    public required string Id { get; set; }
    public required string Name { get; set; }
    public string? Description { get; set; }
    public string? Url { get; set; }
    public List<BoardListResponse> Lists { get; set; } = [];
}
