namespace Artifactory.Infrastructure.Dtos;

// Each search hit is a URI pointing at the item's storage API resource.
internal class ArtifactSearchItemDto
{
    public string Uri { get; set; } = string.Empty;
}
