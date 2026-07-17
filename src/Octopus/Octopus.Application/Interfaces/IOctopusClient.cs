using Octopus.Domain.Entities;

namespace Octopus.Application.Interfaces;

public interface IOctopusClient
{
    Task<List<DeploymentEnvironment>> GetEnvironmentsAsync(CancellationToken cancellationToken = default);

    Task<List<Project>> GetProjectsAsync(CancellationToken cancellationToken = default);

    /// <summary>
    /// Returns the most recent successful deployment to the given environment on or before <paramref name="asOf"/>,
    /// per project. Returns null if the environment name does not resolve to a known Octopus environment.
    /// </summary>
    Task<List<DeployedRelease>?> GetDeployedReleasesAsync(
        string environmentName, DateTimeOffset asOf, string? projectName = null, CancellationToken cancellationToken = default);
}
