namespace Trello.Api.Requests;

public class CreateCardRequest
{
    public required string ListId { get; set; }
    public required string Name { get; set; }
    public string? Description { get; set; }
}
