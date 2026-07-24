namespace Artifactory.Infrastructure.Dtos;

internal class BuildArtifactDto
{
    public string? Name { get; set; }
    public string? Type { get; set; }
    public string? Sha256 { get; set; }
}
