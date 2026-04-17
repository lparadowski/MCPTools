namespace GitHub.Infrastructure.Settings;

public class InfrastructureSettings
{
    public required string GitHubToken { get; set; }
    public bool DisableSslValidation { get; set; }
}
