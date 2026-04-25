namespace Jira.Infrastructure.Settings;

public class InfrastructureSettings
{
    public required string JiraBaseUrl { get; set; }
    public required string JiraEmail { get; set; }
    public required string JiraApiToken { get; set; }
}
