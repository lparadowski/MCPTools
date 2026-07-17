namespace Octopus.Infrastructure.Settings;

public class InfrastructureSettings
{
    public required string OctopusServerUrl { get; set; }
    public required string OctopusApiKey { get; set; }

    /// <summary>
    /// Optional Octopus space name or id. When null/empty the default space is used.
    /// </summary>
    public string? Space { get; set; }

    public bool DisableSslValidation { get; set; }
}
