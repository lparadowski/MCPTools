namespace Miro.Infrastructure.Settings;

public class InfrastructureSettings
{
    public required string MiroAccessToken { get; set; }
    public bool DisableSslValidation { get; set; }
}
