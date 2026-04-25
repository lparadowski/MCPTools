namespace Confluence.Infrastructure.Dtos;

internal class ConfluencePageDto
{
    public string Id { get; set; } = string.Empty;
    public string Title { get; set; } = string.Empty;
    public string SpaceId { get; set; } = string.Empty;
    public string? ParentId { get; set; }
    public string? Status { get; set; }
    public ConfluencePageBodyDto? Body { get; set; }
    public ConfluencePageVersionDto? Version { get; set; }
}
