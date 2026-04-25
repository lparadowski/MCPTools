namespace Trello.Api.Requests;

public class CreateBoardRequest
{
    public required string Name { get; set; }
    public string? Description { get; set; }
}
