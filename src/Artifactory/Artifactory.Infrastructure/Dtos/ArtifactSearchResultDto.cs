namespace Artifactory.Infrastructure.Dtos;

// The Artifactory Artifact Search API returns { "results": [ { "uri": "..." }, ... ] }.
internal class ArtifactSearchResultDto
{
    public List<ArtifactSearchItemDto> Results { get; set; } = [];
}
