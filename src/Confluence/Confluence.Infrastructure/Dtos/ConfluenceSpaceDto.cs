namespace Confluence.Infrastructure.Dtos;

internal class ConfluenceSpaceDto
{
    public string Id { get; set; } = string.Empty;
    public string Key { get; set; } = string.Empty;
    public string Name { get; set; } = string.Empty;
    public ConfluenceSpaceDescriptionDto? Description { get; set; }
    public string? Status { get; set; }
    public string? Type { get; set; }
    public string? HomepageId { get; set; }
}
