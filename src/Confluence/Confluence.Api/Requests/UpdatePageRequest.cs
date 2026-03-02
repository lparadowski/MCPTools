namespace Confluence.Api.Requests;

public class UpdatePageRequest
{
    public required string Title { get; set; }
    public string? Body { get; set; }
    public required int Version { get; set; }
}
