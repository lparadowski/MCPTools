namespace Artifactory.Domain.Entities;

public class BuildDetail
{
    public required string Name { get; set; }
    public required string Number { get; set; }
    public string? Started { get; set; }

    /// <summary>The CI build-run URL (e.g. the GitHub Actions / Azure Pipelines run that produced this build).</summary>
    public string? BuildUrl { get; set; }
    public string? Agent { get; set; }

    /// <summary>
    /// Best-effort source commit SHA, taken from the build-info VCS block when present, otherwise from
    /// well-known CI environment properties (BUILD_SOURCEVERSION, GIT_COMMIT, ...). Null when none is published.
    /// </summary>
    public string? CommitSha { get; set; }
    public string? Branch { get; set; }
    public string? RepoUrl { get; set; }

    /// <summary>Names of the artifacts this build produced (capped).</summary>
    public List<string> Artifacts { get; set; } = [];
}
