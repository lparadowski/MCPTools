namespace Artifactory.Infrastructure.Dtos;

// The storage API with ?properties returns { "uri": "...", "properties": { "key": ["value", ...] } }.
internal class ArtifactPropertiesDto
{
    public string Uri { get; set; } = string.Empty;
    public Dictionary<string, List<string>> Properties { get; set; } = new();
}
