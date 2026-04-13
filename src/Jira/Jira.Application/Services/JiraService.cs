using FluentResults;
using Jira.Application.Interfaces;
using Shared.Application.ResultErrors;
using Jira.Domain.Entities;
using Shared.Application.Chunking;

namespace Jira.Application.Services;

public class JiraService(IJiraClient jiraClient, ChunkingSettings chunkingSettings) : IJiraService
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

    public async Task<Result<ChunkedResult<Issue>>> GetIssueAsync(string issueKeyOrId, int offset = 0, int maxLength = 0, CancellationToken cancellationToken = default)
    {
        var issue = await jiraClient.GetIssueAsync(issueKeyOrId, cancellationToken);

        if (issue is null)
        {
            return Result.Fail<ChunkedResult<Issue>>(new EntityDoesNotExistError());
        }

        var effectiveMaxLength = maxLength > 0 ? maxLength : chunkingSettings.DefaultMaxLength;
        var result = new ChunkedResult<Issue> { Value = issue };

        if (!string.IsNullOrEmpty(issue.Description) && issue.Description.Length > effectiveMaxLength)
        {
            var chunked = ContentChunker.Chunk(issue.Description, offset, effectiveMaxLength);
            issue.Description = chunked.Content;
            result.ChunkMetadata = chunked;
        }

        return Result.Ok(result);
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

    public async Task<Result<ChunkedResult<List<Issue>>>> SearchIssuesAsync(string jql, int offset = 0, int maxLength = 0, int maxResults = 50, CancellationToken cancellationToken = default)
    {
        var issues = await jiraClient.SearchIssuesAsync(jql, maxResults, cancellationToken);
        var effectiveMaxLength = maxLength > 0 ? maxLength : chunkingSettings.DefaultMaxLength;
        return Result.Ok(ContentChunker.ChunkList(issues, offset, effectiveMaxLength));
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

    // Worklogs

    public async Task<Result<List<Worklog>>> GetWorklogsAsync(string issueKeyOrId, CancellationToken cancellationToken = default)
    {
        var worklogs = await jiraClient.GetWorklogsAsync(issueKeyOrId, cancellationToken);
        return Result.Ok(worklogs);
    }

    public async Task<Result<List<Worklog>>> GetUserWorklogsAsync(string username, DateTime startDate, DateTime endDate, CancellationToken cancellationToken = default)
    {
        var worklogs = await jiraClient.GetUserWorklogsAsync(username, startDate, endDate, cancellationToken);
        return Result.Ok(worklogs);
    }

    public async Task<Result<Worklog>> AddWorklogAsync(string issueKeyOrId, string timeSpent, string? comment, DateTime? started, CancellationToken cancellationToken = default)
    {
        var worklog = await jiraClient.AddWorklogAsync(issueKeyOrId, timeSpent, comment, started, cancellationToken);

        if (worklog is null)
        {
            return Result.Fail<Worklog>(new OperationFailedError());
        }

        return Result.Ok(worklog);
    }

    public async Task<Result<Worklog>> UpdateWorklogAsync(string issueKeyOrId, string worklogId, string timeSpent, string? comment, DateTime? started, CancellationToken cancellationToken = default)
    {
        var worklog = await jiraClient.UpdateWorklogAsync(issueKeyOrId, worklogId, timeSpent, comment, started, cancellationToken);

        if (worklog is null)
        {
            return Result.Fail<Worklog>(new OperationFailedError());
        }

        return Result.Ok(worklog);
    }

    public async Task<Result> DeleteWorklogAsync(string issueKeyOrId, string worklogId, CancellationToken cancellationToken = default)
    {
        var success = await jiraClient.DeleteWorklogAsync(issueKeyOrId, worklogId, cancellationToken);

        if (!success)
        {
            return Result.Fail(new OperationFailedError());
        }

        return Result.Ok();
    }

    // Activity

    public async Task<Result<ChunkedResult<List<UserActivity>>>> GetUserActivityAsync(string accountId, DateTime startDate, DateTime endDate, int offset = 0, int maxLength = 0, CancellationToken cancellationToken = default)
    {
        var activity = await jiraClient.GetUserActivityAsync(accountId, startDate, endDate, cancellationToken);
        var effectiveMaxLength = maxLength > 0 ? maxLength : chunkingSettings.DefaultMaxLength;
        return Result.Ok(ContentChunker.ChunkList(activity, offset, effectiveMaxLength));
    }

    // Fields

    public async Task<Result<Dictionary<string, string>>> GetFieldsAsync(CancellationToken cancellationToken = default)
    {
        var fields = await jiraClient.GetFieldsAsync(cancellationToken);
        return Result.Ok(fields);
    }
}
