namespace Artifactory.Api.Responses;

public class ArtifactItemResponse
{
    public required string Repo { get; set; }
    public required string Path { get; set; }
    public required string Name { get; set; }
    public Dictionary<string, List<string>> Properties { get; set; } = new();
}
