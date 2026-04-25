namespace Jira.Infrastructure.Dtos;

public class JiraTransitionDto
{
    public string? Id { get; set; }
    public string? Name { get; set; }
    public JiraStatusDto? To { get; set; }
}
