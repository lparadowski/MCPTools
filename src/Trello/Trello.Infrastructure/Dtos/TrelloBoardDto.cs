namespace Trello.Infrastructure.Dtos;

public class TrelloBoardDto
{
    public string? Id { get; set; }
    public string? Name { get; set; }
    public string? Desc { get; set; }
    public string? Url { get; set; }
    public bool? Closed { get; set; }
}
