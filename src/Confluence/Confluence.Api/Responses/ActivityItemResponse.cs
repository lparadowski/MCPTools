namespace Confluence.Api.Responses;

public class ActivityItemResponse
{
    public required string Type { get; set; }
    public required string Title { get; set; }
    public string? Excerpt { get; set; }
    public string? SpaceKey { get; set; }
    public string? Url { get; set; }
    public DateTime? Date { get; set; }
}
