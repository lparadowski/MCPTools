namespace Confluence.Infrastructure.Dtos;

internal class ConfluencePagesPageDto
{
    public List<ConfluencePageDto>? Results { get; set; }
    public ConfluenceLinksDto? Links { get; set; }
}
