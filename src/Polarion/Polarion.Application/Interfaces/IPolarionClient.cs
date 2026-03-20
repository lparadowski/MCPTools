using Polarion.Domain.Entities;

namespace Polarion.Application.Interfaces;

public interface IPolarionClient
{
    // Projects
    Task<List<Project>> GetProjectsAsync(CancellationToken cancellationToken = default);
    Task<Project?> GetProjectAsync(string projectId, CancellationToken cancellationToken = default);

    // Requirements (Work Items)
    Task<List<Requirement>> GetRequirementsAsync(string projectId, string? query = null, int maxResults = 50, CancellationToken cancellationToken = default);
    Task<Requirement?> GetRequirementAsync(string projectId, string workItemId, CancellationToken cancellationToken = default);

    // Linked Work Items
    Task<List<LinkedWorkItem>> GetLinkedWorkItemsAsync(string projectId, string workItemId, CancellationToken cancellationToken = default);
}
