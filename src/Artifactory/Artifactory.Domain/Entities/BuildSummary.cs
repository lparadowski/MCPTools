namespace Artifactory.Domain.Entities;

public class BuildSummary
{
    public required string Name { get; set; }

    /// <summary>ISO-8601 timestamp of the most recent run of this build, as reported by Artifactory.</summary>
    public string? LastStarted { get; set; }
}
