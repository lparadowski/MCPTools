namespace Artifactory.Domain.Entities;

public class ArtifactItem
{
    public required string Repo { get; set; }
    public required string Path { get; set; }
    public required string Name { get; set; }

    /// <summary>
    /// Artifactory item properties, keyed by property name. This is where build and VCS provenance
    /// lives (e.g. "build.name", "build.number", "vcs.revision", "vcs.url"), when it has been published.
    /// </summary>
    public Dictionary<string, List<string>> Properties { get; set; } = new();
}
