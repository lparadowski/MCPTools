namespace Polarion.Infrastructure.Settings;

public class InfrastructureSettings
{
    public required string PolarionBaseUrl { get; set; }
    public string? PolarionToken { get; set; }
    public string? PolarionUsername { get; set; }
    public string? PolarionPassword { get; set; }
    public bool DisableSslValidation { get; set; }
}
