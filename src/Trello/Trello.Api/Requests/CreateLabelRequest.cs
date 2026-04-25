namespace Trello.Api.Requests;

public class CreateLabelRequest
{
    public required string Name { get; set; }
    public required string Color { get; set; }
}
