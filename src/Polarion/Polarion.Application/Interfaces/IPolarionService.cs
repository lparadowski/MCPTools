using FluentResults;
using Polarion.Domain.Entities;

namespace Polarion.Application.Interfaces;

public interface IPolarionService
{
    // Projects
    Task<Result<List<Project>>> GetProjectsAsync(CancellationToken cancellationToken = default);
    Task<Result<Project>> GetProjectAsync(string projectId, CancellationToken cancellationToken = default);

    // Requirements
    Task<Result<List<Requirement>>> GetRequirementsAsync(string projectId, string? query = null, int maxResults = 50, CancellationToken cancellationToken = default);
    Task<Result<Requirement>> GetRequirementAsync(string projectId, string workItemId, CancellationToken cancellationToken = default);

    // Linked Work Items
    Task<Result<List<LinkedWorkItem>>> GetLinkedWorkItemsAsync(string projectId, string workItemId, CancellationToken cancellationToken = default);
}
