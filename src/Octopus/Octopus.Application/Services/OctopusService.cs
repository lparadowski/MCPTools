using FluentResults;
using Octopus.Application.Interfaces;
using Octopus.Domain.Entities;
using Shared.Application.ResultErrors;

namespace Octopus.Application.Services;

public class OctopusService(IOctopusClient octopusClient) : IOctopusService
{
    public async Task<Result<List<DeploymentEnvironment>>> GetEnvironmentsAsync(CancellationToken cancellationToken = default)
    {
        var environments = await octopusClient.GetEnvironmentsAsync(cancellationToken);
        return Result.Ok(environments);
    }

    public async Task<Result<List<Project>>> GetProjectsAsync(CancellationToken cancellationToken = default)
    {
        var projects = await octopusClient.GetProjectsAsync(cancellationToken);
        return Result.Ok(projects);
    }

    public async Task<Result<List<DeployedRelease>>> GetDeployedReleasesAsync(
        string environmentName, DateTimeOffset? asOf = null, string? projectName = null, CancellationToken cancellationToken = default)
    {
        if (string.IsNullOrWhiteSpace(environmentName))
        {
            return Result.Fail<List<DeployedRelease>>(
                new ValidationError(nameof(environmentName), ["An environment name is required."]));
        }

        var releases = await octopusClient.GetDeployedReleasesAsync(
            environmentName, asOf ?? DateTimeOffset.UtcNow, projectName, cancellationToken);

        if (releases is null)
        {
            return Result.Fail<List<DeployedRelease>>(
                new ValidationError(nameof(environmentName), [$"Environment '{environmentName}' was not found in Octopus."]));
        }

        return Result.Ok(releases);
    }
}
