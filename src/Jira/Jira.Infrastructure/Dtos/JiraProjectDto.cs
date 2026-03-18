namespace Jira.Infrastructure.Dtos;

public class JiraProjectDto
{
    public string? Id { get; set; }
    public string? Key { get; set; }
    public string? Name { get; set; }
    public string? Description { get; set; }
    public string? ProjectTypeKey { get; set; }
    public string? Style { get; set; }
    public JiraUserDto? Lead { get; set; }
    public string? Self { get; set; }
}
