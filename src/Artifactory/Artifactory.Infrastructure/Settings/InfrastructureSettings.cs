namespace Artifactory.Infrastructure.Settings;

public class InfrastructureSettings
{
    public required string ArtifactoryBaseUrl { get; set; }
    public required string ArtifactoryPat { get; set; }

    public bool DisableSslValidation { get; set; }
}
