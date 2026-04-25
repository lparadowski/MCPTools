namespace Confluence.Domain.Entities;

public class Page
{
    public required string Id { get; set; }
    public required string Title { get; set; }
    public required string SpaceId { get; set; }
    public string? ParentId { get; set; }
    public string? Status { get; set; }
    public string? Body { get; set; }
    public int Version { get; set; }
}
