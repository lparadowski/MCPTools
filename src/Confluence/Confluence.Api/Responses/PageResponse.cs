namespace Confluence.Api.Responses;

public class PageResponse
{
    public required string Id { get; set; }
    public required string Title { get; set; }
    public required string SpaceId { get; set; }
    public string? ParentId { get; set; }
    public string? Status { get; set; }
    public string? Body { get; set; }
    public int Version { get; set; }
    public int? TotalBodyLength { get; set; }
    public bool? HasMore { get; set; }
    public int? NextOffset { get; set; }
}
