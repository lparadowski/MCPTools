namespace Artifactory.Infrastructure.Dtos;

// GET /api/build returns { "builds": [ { "uri": "/name", "lastStarted": "..." }, ... ] }.
internal class BuildsListDto
{
    public List<BuildRefDto> Builds { get; set; } = [];
}
