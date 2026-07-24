namespace Artifactory.Infrastructure.Dtos;

// GET /api/build/{name}/{number} returns { "buildInfo": { ... } }.
internal class BuildInfoDto
{
    public BuildInfoBodyDto? BuildInfo { get; set; }
}
