namespace Trello.Infrastructure.Dtos;

public class TrelloCardDto
{
    public string? Id { get; set; }
    public string? Name { get; set; }
    public string? Desc { get; set; }
    public string? IdList { get; set; }
    public string? ShortUrl { get; set; }
    public DateTime? DateLastActivity { get; set; }
    public List<TrelloLabelDto>? Labels { get; set; }
}
