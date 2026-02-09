namespace Miro.Api.Responses;

public class BoardResponse
{
    public required string Id { get; set; }
    public required string Name { get; set; }
    public string? Description { get; set; }
    public DateTime? CreatedAt { get; set; }
    public DateTime? ModifiedAt { get; set; }
}
