namespace Artifactory.Infrastructure.Dtos;

internal class ModuleDto
{
    public string? Id { get; set; }
    public List<BuildArtifactDto>? Artifacts { get; set; }
}
