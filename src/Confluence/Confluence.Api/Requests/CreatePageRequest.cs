namespace Confluence.Api.Requests;

public class CreatePageRequest
{
    public required string SpaceId { get; set; }
    public required string Title { get; set; }
    public string? Body { get; set; }
    public string? ParentId { get; set; }
}
