namespace Confluence.Infrastructure.Dtos;

internal class ConfluenceSearchResponseDto
{
    public List<ConfluenceSearchResultDto>? Results { get; set; }
    public int? TotalSize { get; set; }
    public int? Size { get; set; }
}
