using AzureDevOps.Application.Interfaces;
using AzureDevOps.Application.ResultErrors;
using AzureDevOps.Domain.Entities;
using FluentResults;

namespace AzureDevOps.Application.Services;

public class AzureDevOpsService(IAzureDevOpsClient azureDevOpsClient) : IAzureDevOpsService
{
    // Projects
    public async Task<Result<List<Project>>> GetProjectsAsync(CancellationToken cancellationToken = default)
    {
        var projects = await azureDevOpsClient.GetProjectsAsync(cancellationToken);
        return Result.Ok(projects);
    }

    public async Task<Result<Project>> GetProjectAsync(string projectNameOrId, CancellationToken cancellationToken = default)
    {
        var project = await azureDevOpsClient.GetProjectAsync(projectNameOrId, cancellationToken);

        if (project is null)
        {
            return Result.Fail<Project>(new EntityDoesNotExistError());
        }

        return Result.Ok(project);
    }

    // Work Items
    public async Task<Result<WorkItem>> GetWorkItemAsync(int id, CancellationToken cancellationToken = default)
    {
        var workItem = await azureDevOpsClient.GetWorkItemAsync(id, cancellationToken);

        if (workItem is null)
        {
            return Result.Fail<WorkItem>(new EntityDoesNotExistError());
        }

        return Result.Ok(workItem);
    }

    public async Task<Result<WorkItem>> CreateWorkItemAsync(string project, string workItemType, string title, string? description, int? parentId, string? assignedTo, string? areaPath, string? iterationPath, string? priority, string? tags, CancellationToken cancellationToken = default)
    {
        var workItem = await azureDevOpsClient.CreateWorkItemAsync(project, workItemType, title, description, parentId, assignedTo, areaPath, iterationPath, priority, tags, cancellationToken);

        if (workItem is null)
        {
            return Result.Fail<WorkItem>(new OperationFailedError());
        }

        return Result.Ok(workItem);
    }

    public async Task<Result<WorkItem>> UpdateWorkItemAsync(int id, string? title, string? description, string? state, string? assignedTo, string? areaPath, string? iterationPath, string? priority, string? tags, CancellationToken cancellationToken = default)
    {
        var workItem = await azureDevOpsClient.UpdateWorkItemAsync(id, title, description, state, assignedTo, areaPath, iterationPath, priority, tags, cancellationToken);

        if (workItem is null)
        {
            return Result.Fail<WorkItem>(new EntityDoesNotExistError());
        }

        return Result.Ok(workItem);
    }

    public async Task<Result> DeleteWorkItemAsync(int id, CancellationToken cancellationToken = default)
    {
        var success = await azureDevOpsClient.DeleteWorkItemAsync(id, cancellationToken);

        if (!success)
        {
            return Result.Fail(new OperationFailedError());
        }

        return Result.Ok();
    }

    public async Task<Result<List<WorkItem>>> QueryWorkItemsAsync(string wiql, CancellationToken cancellationToken = default)
    {
        var workItems = await azureDevOpsClient.QueryWorkItemsAsync(wiql, cancellationToken);
        return Result.Ok(workItems);
    }

    // Teams
    public async Task<Result<List<Team>>> GetTeamsAsync(string project, CancellationToken cancellationToken = default)
    {
        var teams = await azureDevOpsClient.GetTeamsAsync(project, cancellationToken);
        return Result.Ok(teams);
    }

    // Boards
    public async Task<Result<List<Board>>> GetBoardsAsync(string project, string team, CancellationToken cancellationToken = default)
    {
        var boards = await azureDevOpsClient.GetBoardsAsync(project, team, cancellationToken);
        return Result.Ok(boards);
    }

    // Sprints
    public async Task<Result<List<Sprint>>> GetSprintsAsync(string project, string team, CancellationToken cancellationToken = default)
    {
        var sprints = await azureDevOpsClient.GetSprintsAsync(project, team, cancellationToken);
        return Result.Ok(sprints);
    }

    // Comments
    public async Task<Result<List<Comment>>> GetCommentsAsync(string project, int workItemId, CancellationToken cancellationToken = default)
    {
        var comments = await azureDevOpsClient.GetCommentsAsync(project, workItemId, cancellationToken);
        return Result.Ok(comments);
    }

    public async Task<Result<Comment>> AddCommentAsync(string project, int workItemId, string text, CancellationToken cancellationToken = default)
    {
        var comment = await azureDevOpsClient.AddCommentAsync(project, workItemId, text, cancellationToken);

        if (comment is null)
        {
            return Result.Fail<Comment>(new OperationFailedError());
        }

        return Result.Ok(comment);
    }
}
