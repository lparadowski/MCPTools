using AzureDevOps.Domain.Entities;

namespace AzureDevOps.Application.Interfaces;

public interface IAzureDevOpsClient
{
    // Projects
    Task<List<Project>> GetProjectsAsync(CancellationToken cancellationToken = default);
    Task<Project?> GetProjectAsync(string projectNameOrId, CancellationToken cancellationToken = default);

    // Work Items
    Task<WorkItem?> GetWorkItemAsync(int id, CancellationToken cancellationToken = default);
    Task<WorkItem?> CreateWorkItemAsync(string project, string workItemType, string title, string? description, int? parentId, string? assignedTo, string? areaPath, string? iterationPath, string? priority, string? tags, CancellationToken cancellationToken = default);
    Task<WorkItem?> UpdateWorkItemAsync(int id, string? title, string? description, string? state, string? assignedTo, string? areaPath, string? iterationPath, string? priority, string? tags, CancellationToken cancellationToken = default);
    Task<bool> DeleteWorkItemAsync(int id, CancellationToken cancellationToken = default);
    Task<List<WorkItem>> QueryWorkItemsAsync(string wiql, CancellationToken cancellationToken = default);

    // Teams
    Task<List<Team>> GetTeamsAsync(string project, CancellationToken cancellationToken = default);

    // Boards
    Task<List<Board>> GetBoardsAsync(string project, string team, CancellationToken cancellationToken = default);

    // Sprints (Iterations)
    Task<List<Sprint>> GetSprintsAsync(string project, string team, CancellationToken cancellationToken = default);

    // Comments
    Task<List<Comment>> GetCommentsAsync(string project, int workItemId, CancellationToken cancellationToken = default);
    Task<Comment?> AddCommentAsync(string project, int workItemId, string text, CancellationToken cancellationToken = default);
}
