namespace Confluence.Domain.Entities;

public class SearchResult
{
    public required string Title { get; set; }
    public string? Type { get; set; }
    public string? Id { get; set; }
    public string? SpaceKey { get; set; }
    public string? Excerpt { get; set; }
    public string? Url { get; set; }
    public DateTime? LastModified { get; set; }
}
