namespace Confluence.Infrastructure.Dtos;

internal class ConfluenceSpacesPageDto
{
    public List<ConfluenceSpaceDto>? Results { get; set; }
    public ConfluenceLinksDto? Links { get; set; }
}
