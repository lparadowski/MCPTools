namespace Trello.Infrastructure.Dtos;

public class TrelloActionDto
{
    public string? Id { get; set; }
    public TrelloActionDataDto? Data { get; set; }
    public DateTime? Date { get; set; }
}
