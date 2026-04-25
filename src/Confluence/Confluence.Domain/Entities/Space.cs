namespace Confluence.Domain.Entities;

public class Space
{
    public required string Id { get; set; }
    public required string Key { get; set; }
    public required string Name { get; set; }
    public string? Description { get; set; }
    public string? Status { get; set; }
    public string? Type { get; set; }
    public string? HomepageId { get; set; }
}
