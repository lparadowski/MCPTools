using Artifactory.Domain.Entities;

namespace Artifactory.Application.Interfaces;

public interface IArtifactoryClient
{
    /// <summary>
    /// Searches Artifactory for artifacts whose file name matches <paramref name="name"/> and returns each
    /// match enriched with its properties. Optionally constrained to a single <paramref name="repo"/>.
    /// </summary>
    Task<List<ArtifactItem>> SearchArtifactsAsync(
        string name, string? repo = null, CancellationToken cancellationToken = default);

    /// <summary>
    /// Returns all published build names, optionally filtered to those whose name contains <paramref name="filter"/>.
    /// </summary>
    Task<List<BuildSummary>> GetBuildsAsync(string? filter = null, CancellationToken cancellationToken = default);

    /// <summary>
    /// Returns the normalized build-info for a build. When <paramref name="number"/> is null the latest run is used.
    /// Returns null when the build (or run) does not exist.
    /// </summary>
    Task<BuildDetail?> GetBuildInfoAsync(string name, string? number = null, CancellationToken cancellationToken = default);
}
