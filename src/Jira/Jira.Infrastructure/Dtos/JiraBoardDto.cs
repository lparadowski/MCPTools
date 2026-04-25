namespace Jira.Infrastructure.Dtos;

public class JiraBoardDto
{
    public int Id { get; set; }
    public string? Name { get; set; }
    public string? Type { get; set; }
    public JiraBoardLocationDto? Location { get; set; }
}
