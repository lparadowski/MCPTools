namespace Confluence.Infrastructure.Dtos;

internal class ConfluenceSearchContentDto
{
    public string? Id { get; set; }
    public string? Type { get; set; }
    public string? Title { get; set; }
    public ConfluenceSearchSpaceDto? Space { get; set; }
}
