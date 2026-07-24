using FluentResults;
using Artifactory.Application.Interfaces;
using Artifactory.Domain.Entities;
using Shared.Application.ResultErrors;

namespace Artifactory.Application.Services;

public class ArtifactoryService(IArtifactoryClient artifactoryClient) : IArtifactoryService
{
    public async Task<Result<List<ArtifactItem>>> SearchArtifactsAsync(
        string name, string? repo = null, CancellationToken cancellationToken = default)
    {
        if (string.IsNullOrWhiteSpace(name))
        {
            return Result.Fail<List<ArtifactItem>>(
                new ValidationError(nameof(name), ["An artifact name to search for is required."]));
        }

        var items = await artifactoryClient.SearchArtifactsAsync(name, repo, cancellationToken);
        return Result.Ok(items);
    }

    public async Task<Result<List<BuildSummary>>> GetBuildsAsync(
        string? filter = null, CancellationToken cancellationToken = default)
    {
        var builds = await artifactoryClient.GetBuildsAsync(filter, cancellationToken);
        return Result.Ok(builds);
    }

    public async Task<Result<BuildDetail>> GetBuildInfoAsync(
        string name, string? number = null, CancellationToken cancellationToken = default)
    {
        if (string.IsNullOrWhiteSpace(name))
        {
            return Result.Fail<BuildDetail>(
                new ValidationError(nameof(name), ["A build name is required."]));
        }

        var detail = await artifactoryClient.GetBuildInfoAsync(name, number, cancellationToken);

        if (detail is null)
        {
            return Result.Fail<BuildDetail>(
                new ValidationError(nameof(name), [$"No build-info was found for build '{name}'."]));
        }

        return Result.Ok(detail);
    }
}
