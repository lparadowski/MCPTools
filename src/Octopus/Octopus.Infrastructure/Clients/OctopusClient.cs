using System.Net.Http.Json;
using Octopus.Application.Interfaces;
using Octopus.Domain.Entities;
using Octopus.Infrastructure.Dtos;
using Octopus.Infrastructure.Settings;

namespace Octopus.Infrastructure.Clients;

public class OctopusClient(IHttpClientFactory httpClientFactory, InfrastructureSettings settings) : IOctopusClient
{
    // Guard so a very old asOf against a busy environment doesn't page forever.
    private const int PageSize = 200;
    private const int MaxItems = 1000;

    public async Task<List<DeploymentEnvironment>> GetEnvironmentsAsync(CancellationToken cancellationToken = default)
    {
        var http = httpClientFactory.CreateClient("OctopusApi");
        var spaceId = await ResolveSpaceIdAsync(http, cancellationToken);

        var environments = await http.GetFromJsonAsync<List<OctopusEnvironmentDto>>(
            $"/api/{spaceId}/environments/all", cancellationToken) ?? [];

        return environments
            .Select(e => new DeploymentEnvironment { Id = e.Id, Name = e.Name })
            .ToList();
    }

    public async Task<List<Project>> GetProjectsAsync(CancellationToken cancellationToken = default)
    {
        var http = httpClientFactory.CreateClient("OctopusApi");
        var spaceId = await ResolveSpaceIdAsync(http, cancellationToken);

        var projects = await http.GetFromJsonAsync<List<OctopusProjectDto>>(
            $"/api/{spaceId}/projects/all", cancellationToken) ?? [];

        return projects
            .Select(p => new Project { Id = p.Id, Name = p.Name })
            .OrderBy(p => p.Name)
            .ToList();
    }

    public async Task<List<DeployedRelease>?> GetDeployedReleasesAsync(
        string environmentName, DateTimeOffset asOf, string? projectName = null, CancellationToken cancellationToken = default)
    {
        var http = httpClientFactory.CreateClient("OctopusApi");
        var spaceId = await ResolveSpaceIdAsync(http, cancellationToken);

        var environments = await http.GetFromJsonAsync<List<OctopusEnvironmentDto>>(
            $"/api/{spaceId}/environments/all", cancellationToken) ?? [];
        var environment = environments.FirstOrDefault(e =>
            e.Name.Equals(environmentName, StringComparison.OrdinalIgnoreCase));

        if (environment is null)
        {
            return null;
        }

        var projects = await http.GetFromJsonAsync<List<OctopusProjectDto>>(
            $"/api/{spaceId}/projects/all", cancellationToken) ?? [];
        var projectNamesById = projects.ToDictionary(p => p.Id, p => p.Name);

        string? projectId = null;
        if (!string.IsNullOrWhiteSpace(projectName))
        {
            var project = projects.FirstOrDefault(p =>
                p.Name.Equals(projectName, StringComparison.OrdinalIgnoreCase));

            // Unknown project => nothing was deployed for it. Environment is the anchor, so return empty rather than "not found".
            if (project is null)
            {
                return [];
            }

            projectId = project.Id;
        }

        var deployments = await GetSuccessfulDeploymentsAsync(http, spaceId, environment.Id, projectId, cancellationToken);

        // Most recent successful deployment per project, on or before asOf.
        var latestPerProject = deployments
            .Where(d => d.Created <= asOf)
            .GroupBy(d => d.ProjectId)
            .Select(g => g.OrderByDescending(d => d.Created).First())
            .ToList();

        var results = new List<DeployedRelease>();
        foreach (var deployment in latestPerProject)
        {
            var version = await GetReleaseVersionAsync(http, spaceId, deployment.ReleaseId, cancellationToken);

            results.Add(new DeployedRelease
            {
                ProjectId = deployment.ProjectId,
                ProjectName = projectNamesById.TryGetValue(deployment.ProjectId, out var name) ? name : deployment.ProjectId,
                EnvironmentId = environment.Id,
                EnvironmentName = environment.Name,
                ReleaseId = deployment.ReleaseId,
                Version = version ?? string.Empty,
                DeploymentId = deployment.Id,
                DeployedAt = deployment.Created,
                State = "Success"
            });
        }

        return results.OrderBy(r => r.ProjectName).ToList();
    }

    private async Task<string> ResolveSpaceIdAsync(HttpClient http, CancellationToken cancellationToken)
    {
        var spaces = await http.GetFromJsonAsync<List<OctopusSpaceDto>>("/api/spaces/all", cancellationToken) ?? [];

        OctopusSpaceDto? space = null;
        if (!string.IsNullOrWhiteSpace(settings.Space))
        {
            space = spaces.FirstOrDefault(s =>
                s.Name.Equals(settings.Space, StringComparison.OrdinalIgnoreCase) ||
                s.Id.Equals(settings.Space, StringComparison.OrdinalIgnoreCase));
        }

        space ??= spaces.FirstOrDefault(s => s.IsDefault) ?? spaces.FirstOrDefault();

        // "Spaces-1" is the id of the default space on a stock Octopus install.
        return space?.Id ?? "Spaces-1";
    }

    private static async Task<List<OctopusDeploymentDto>> GetSuccessfulDeploymentsAsync(
        HttpClient http, string spaceId, string environmentId, string? projectId, CancellationToken cancellationToken)
    {
        var baseUrl = $"/api/{spaceId}/deployments?environments={environmentId}&taskState=Success";
        if (projectId is not null)
        {
            baseUrl += $"&projects={projectId}";
        }

        var deployments = new List<OctopusDeploymentDto>();
        for (var skip = 0; skip < MaxItems; skip += PageSize)
        {
            var page = await http.GetFromJsonAsync<OctopusCollectionDto<OctopusDeploymentDto>>(
                $"{baseUrl}&take={PageSize}&skip={skip}", cancellationToken);

            var items = page?.Items ?? [];
            deployments.AddRange(items);

            if (items.Count < PageSize)
            {
                break;
            }
        }

        return deployments;
    }

    private static async Task<string?> GetReleaseVersionAsync(
        HttpClient http, string spaceId, string releaseId, CancellationToken cancellationToken)
    {
        var response = await http.GetAsync($"/api/{spaceId}/releases/{releaseId}", cancellationToken);
        if (!response.IsSuccessStatusCode)
        {
            return null;
        }

        var release = await response.Content.ReadFromJsonAsync<OctopusReleaseDto>(cancellationToken: cancellationToken);
        return release?.Version;
    }
}
