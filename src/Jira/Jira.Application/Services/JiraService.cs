using FluentResults;
using Jira.Application.Interfaces;
using Jira.Application.ResultErrors;
using Jira.Domain.Entities;

namespace Jira.Application.Services;

public class JiraService(IJiraClient jiraClient) : IJiraService
{
    // Projects

    public async Task<Result<List<Project>>> GetProjectsAsync(CancellationToken cancellationToken = default)
    {
        var projects = await jiraClient.GetProjectsAsync(cancellationToken);
        return Result.Ok(projects);
    }

    public async Task<Result<Project>> GetProjectAsync(string projectKeyOrId, CancellationToken cancellationToken = default)
    {
        var project = await jiraClient.GetProjectAsync(projectKeyOrId, cancellationToken);

        if (project is null)
        {
            return Result.Fail<Project>(new EntityDoesNotExistError());
        }

        return Result.Ok(project);
    }

    // Issues

    public async Task<Result<Issue>> CreateIssueAsync(
        string projectKey,
        string issueType,
        string summary,
        string? description,
        Dictionary<string, string?>? customFields,
        string? parentKey,
        List<string>? labels,
        CancellationToken cancellationToken = default)
    {
        var issue = await jiraClient.CreateIssueAsync(projectKey, issueType, summary, description, customFields, parentKey, labels, cancellationToken);

        if (issue is null)
        {
            return Result.Fail<Issue>(new OperationFailedError());
        }

        return Result.Ok(issue);
    }

    public async Task<Result<Issue>> GetIssueAsync(string issueKeyOrId, CancellationToken cancellationToken = default)
    {
        var issue = await jiraClient.GetIssueAsync(issueKeyOrId, cancellationToken);

        if (issue is null)
        {
            return Result.Fail<Issue>(new EntityDoesNotExistError());
        }

        return Result.Ok(issue);
    }

    public async Task<Result<Issue>> UpdateIssueAsync(
        string issueKeyOrId,
        string? summary,
        string? description,
        Dictionary<string, string?>? customFields,
        CancellationToken cancellationToken = default)
    {
        var issue = await jiraClient.UpdateIssueAsync(issueKeyOrId, summary, description, customFields, cancellationToken);

        if (issue is null)
        {
            return Result.Fail<Issue>(new EntityDoesNotExistError());
        }

        return Result.Ok(issue);
    }

    public async Task<Result> DeleteIssueAsync(string issueKeyOrId, CancellationToken cancellationToken = default)
    {
        var success = await jiraClient.DeleteIssueAsync(issueKeyOrId, cancellationToken);

        if (!success)
        {
            return Result.Fail(new EntityDoesNotExistError());
        }

        return Result.Ok();
    }

    public async Task<Result<List<Issue>>> SearchIssuesAsync(string jql, int maxResults = 50, CancellationToken cancellationToken = default)
    {
        var issues = await jiraClient.SearchIssuesAsync(jql, maxResults, cancellationToken);
        return Result.Ok(issues);
    }

    // Transitions

    public async Task<Result<List<Transition>>> GetTransitionsAsync(string issueKeyOrId, CancellationToken cancellationToken = default)
    {
        var transitions = await jiraClient.GetTransitionsAsync(issueKeyOrId, cancellationToken);
        return Result.Ok(transitions);
    }

    public async Task<Result> TransitionIssueAsync(string issueKeyOrId, string transitionId, CancellationToken cancellationToken = default)
    {
        var success = await jiraClient.TransitionIssueAsync(issueKeyOrId, transitionId, cancellationToken);

        if (!success)
        {
            return Result.Fail(new OperationFailedError());
        }

        return Result.Ok();
    }

    // Comments

    public async Task<Result<List<Comment>>> GetCommentsAsync(string issueKeyOrId, CancellationToken cancellationToken = default)
    {
        var comments = await jiraClient.GetCommentsAsync(issueKeyOrId, cancellationToken);
        return Result.Ok(comments);
    }

    public async Task<Result<Comment>> AddCommentAsync(string issueKeyOrId, string body, CancellationToken cancellationToken = default)
    {
        var comment = await jiraClient.AddCommentAsync(issueKeyOrId, body, cancellationToken);

        if (comment is null)
        {
            return Result.Fail<Comment>(new OperationFailedError());
        }

        return Result.Ok(comment);
    }

    // Labels

    public async Task<Result> AddLabelAsync(string issueKeyOrId, string label, CancellationToken cancellationToken = default)
    {
        var success = await jiraClient.AddLabelAsync(issueKeyOrId, label, cancellationToken);

        if (!success)
        {
            return Result.Fail(new OperationFailedError());
        }

        return Result.Ok();
    }

    public async Task<Result> RemoveLabelAsync(string issueKeyOrId, string label, CancellationToken cancellationToken = default)
    {
        var success = await jiraClient.RemoveLabelAsync(issueKeyOrId, label, cancellationToken);

        if (!success)
        {
            return Result.Fail(new OperationFailedError());
        }

        return Result.Ok();
    }

    // Assignment

    public async Task<Result> AssignIssueAsync(string issueKeyOrId, string? accountId, CancellationToken cancellationToken = default)
    {
        var success = await jiraClient.AssignIssueAsync(issueKeyOrId, accountId, cancellationToken);

        if (!success)
        {
            return Result.Fail(new OperationFailedError());
        }

        return Result.Ok();
    }

    // Issue Links

    public async Task<Result> LinkIssuesAsync(string inwardIssueKey, string outwardIssueKey, string linkTypeName, CancellationToken cancellationToken = default)
    {
        var success = await jiraClient.LinkIssuesAsync(inwardIssueKey, outwardIssueKey, linkTypeName, cancellationToken);

        if (!success)
        {
            return Result.Fail(new OperationFailedError());
        }

        return Result.Ok();
    }

    // Boards

    public async Task<Result<List<Board>>> GetBoardsAsync(CancellationToken cancellationToken = default)
    {
        var boards = await jiraClient.GetBoardsAsync(cancellationToken);
        return Result.Ok(boards);
    }

    public async Task<Result<Board>> GetBoardAsync(int boardId, CancellationToken cancellationToken = default)
    {
        var board = await jiraClient.GetBoardAsync(boardId, cancellationToken);

        if (board is null)
        {
            return Result.Fail<Board>(new EntityDoesNotExistError());
        }

        return Result.Ok(board);
    }

    // Sprints

    public async Task<Result<List<Sprint>>> GetSprintsAsync(int boardId, CancellationToken cancellationToken = default)
    {
        var sprints = await jiraClient.GetSprintsAsync(boardId, cancellationToken);
        return Result.Ok(sprints);
    }

    public async Task<Result> MoveIssuesToSprintAsync(int sprintId, List<string> issueKeys, CancellationToken cancellationToken = default)
    {
        var success = await jiraClient.MoveIssuesToSprintAsync(sprintId, issueKeys, cancellationToken);

        if (!success)
        {
            return Result.Fail(new OperationFailedError());
        }

        return Result.Ok();
    }
}
