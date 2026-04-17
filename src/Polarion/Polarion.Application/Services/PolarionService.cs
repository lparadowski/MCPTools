using FluentResults;
using Polarion.Application.Interfaces;
using Shared.Application.ResultErrors;
using Polarion.Domain.Entities;

namespace Polarion.Application.Services;

public class PolarionService(IPolarionClient polarionClient) : IPolarionService
{
    // Projects

    public async Task<Result<List<Project>>> GetProjectsAsync(CancellationToken cancellationToken = default)
    {
        var projects = await polarionClient.GetProjectsAsync(cancellationToken);
        return Result.Ok(projects);
    }

    public async Task<Result<Project>> GetProjectAsync(string projectId, CancellationToken cancellationToken = default)
    {
        var project = await polarionClient.GetProjectAsync(projectId, cancellationToken);

        if (project is null)
        {
            return Result.Fail<Project>(new EntityDoesNotExistError());
        }

        return Result.Ok(project);
    }

    // Requirements

    public async Task<Result<List<Requirement>>> GetRequirementsAsync(string projectId, string? query = null, int maxResults = 50, CancellationToken cancellationToken = default)
    {
        var requirements = await polarionClient.GetRequirementsAsync(projectId, query, maxResults, cancellationToken);
        return Result.Ok(requirements);
    }

    public async Task<Result<Requirement>> GetRequirementAsync(string projectId, string workItemId, CancellationToken cancellationToken = default)
    {
        var requirement = await polarionClient.GetRequirementAsync(projectId, workItemId, cancellationToken);

        if (requirement is null)
        {
            return Result.Fail<Requirement>(new EntityDoesNotExistError());
        }

        return Result.Ok(requirement);
    }

    // Document Work Items

    public async Task<Result<List<Requirement>>> GetDocumentWorkItemsAsync(string projectId, string spaceId, string documentName, int maxResults = 50, CancellationToken cancellationToken = default)
    {
        var requirements = await polarionClient.GetDocumentWorkItemsAsync(projectId, spaceId, documentName, maxResults, cancellationToken);
        return Result.Ok(requirements);
    }

    // Linked Work Items

    public async Task<Result<List<LinkedWorkItem>>> GetLinkedWorkItemsAsync(string projectId, string workItemId, CancellationToken cancellationToken = default)
    {
        var linkedItems = await polarionClient.GetLinkedWorkItemsAsync(projectId, workItemId, cancellationToken);
        return Result.Ok(linkedItems);
    }
}
