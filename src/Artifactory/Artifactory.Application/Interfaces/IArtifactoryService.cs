using FluentResults;
using Artifactory.Domain.Entities;

namespace Artifactory.Application.Interfaces;

public interface IArtifactoryService
{
    Task<Result<List<ArtifactItem>>> SearchArtifactsAsync(
        string name, string? repo = null, CancellationToken cancellationToken = default);

    Task<Result<List<BuildSummary>>> GetBuildsAsync(
        string? filter = null, CancellationToken cancellationToken = default);

    Task<Result<BuildDetail>> GetBuildInfoAsync(
        string name, string? number = null, CancellationToken cancellationToken = default);
}
