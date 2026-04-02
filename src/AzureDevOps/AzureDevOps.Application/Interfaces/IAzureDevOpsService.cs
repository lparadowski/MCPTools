using AzureDevOps.Domain.Entities;
using FluentResults;

namespace AzureDevOps.Application.Interfaces;

public interface IAzureDevOpsService
{
    // Projects
    Task<Result<List<Project>>> GetProjectsAsync(CancellationToken cancellationToken = default);
    Task<Result<Project>> GetProjectAsync(string projectNameOrId, CancellationToken cancellationToken = default);

    // Work Items
    Task<Result<WorkItem>> GetWorkItemAsync(int id, CancellationToken cancellationToken = default);
    Task<Result<WorkItem>> CreateWorkItemAsync(string project, string workItemType, string title, string? description, int? parentId, string? assignedTo, string? areaPath, string? iterationPath, string? priority, string? tags, CancellationToken cancellationToken = default);
    Task<Result<WorkItem>> UpdateWorkItemAsync(int id, string? title, string? description, string? state, string? assignedTo, string? areaPath, string? iterationPath, string? priority, string? tags, CancellationToken cancellationToken = default);
    Task<Result> DeleteWorkItemAsync(int id, CancellationToken cancellationToken = default);
    Task<Result<List<WorkItem>>> QueryWorkItemsAsync(string wiql, CancellationToken cancellationToken = default);

    // Teams
    Task<Result<List<Team>>> GetTeamsAsync(string project, CancellationToken cancellationToken = default);

    // Boards
    Task<Result<List<Board>>> GetBoardsAsync(string project, string team, CancellationToken cancellationToken = default);

    // Sprints
    Task<Result<List<Sprint>>> GetSprintsAsync(string project, string team, CancellationToken cancellationToken = default);

    // Comments
    Task<Result<List<Comment>>> GetCommentsAsync(string project, int workItemId, CancellationToken cancellationToken = default);
    Task<Result<Comment>> AddCommentAsync(string project, int workItemId, string text, CancellationToken cancellationToken = default);
}
