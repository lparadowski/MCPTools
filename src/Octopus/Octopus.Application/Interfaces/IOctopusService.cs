using FluentResults;
using Octopus.Domain.Entities;

namespace Octopus.Application.Interfaces;

public interface IOctopusService
{
    Task<Result<List<DeploymentEnvironment>>> GetEnvironmentsAsync(CancellationToken cancellationToken = default);

    Task<Result<List<Project>>> GetProjectsAsync(CancellationToken cancellationToken = default);

    Task<Result<List<DeployedRelease>>> GetDeployedReleasesAsync(
        string environmentName, DateTimeOffset? asOf = null, string? projectName = null, CancellationToken cancellationToken = default);
}
