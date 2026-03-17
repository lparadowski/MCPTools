namespace Jira.Infrastructure.Dtos;

public class JiraUserDto
{
    public string? AccountId { get; set; }
    public string? DisplayName { get; set; }
    public string? EmailAddress { get; set; }
    public bool? Active { get; set; }
}
