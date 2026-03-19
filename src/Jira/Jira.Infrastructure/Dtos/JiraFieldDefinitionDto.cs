namespace Jira.Infrastructure.Dtos;

public class JiraFieldDefinitionDto
{
    public string? Id { get; set; }
    public string? Name { get; set; }
    public bool? Custom { get; set; }
    public JiraFieldSchemaDto? Schema { get; set; }
}
