namespace Artifactory.Infrastructure.Dtos;

// GET /api/build/{name} returns { "buildsNumbers": [ { "uri": "/123", "started": "..." }, ... ] }.
internal class BuildRunsDto
{
    public List<BuildRunDto> BuildsNumbers { get; set; } = [];
}
