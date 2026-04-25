namespace Confluence.Infrastructure.Dtos;

internal class ConfluenceSearchResultDto
{
    public string? Title { get; set; }
    public string? Excerpt { get; set; }
    public string? Url { get; set; }
    public string? EntityType { get; set; }
    public DateTime? LastModified { get; set; }
    public ConfluenceSearchContentDto? Content { get; set; }
}
