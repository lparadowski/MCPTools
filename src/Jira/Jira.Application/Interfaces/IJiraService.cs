using FluentResults;
using Jira.Domain.Entities;

namespace Jira.Application.Interfaces;

public interface IJiraService
{
    // Projects
    Task<Result<List<Project>>> GetProjectsAsync(CancellationToken cancellationToken = default);
    Task<Result<Project>> GetProjectAsync(string projectKeyOrId, CancellationToken cancellationToken = default);

    // Issues
    Task<Result<Issue>> CreateIssueAsync(
        string projectKey,
        string issueType,
        string summary,
        string? description,
        Dictionary<string, string?>? customFields,
        string? parentKey,
        List<string>? labels,
        CancellationToken cancellationToken = default);
    Task<Result<Issue>> GetIssueAsync(string issueKeyOrId, CancellationToken cancellationToken = default);
    Task<Result<Issue>> UpdateIssueAsync(
        string issueKeyOrId,
        string? summary,
        string? description,
        Dictionary<string, string?>? customFields,
        CancellationToken cancellationToken = default);
    Task<Result> DeleteIssueAsync(string issueKeyOrId, CancellationToken cancellationToken = default);
    Task<Result<List<Issue>>> SearchIssuesAsync(string jql, int maxResults = 50, CancellationToken cancellationToken = default);

    // Transitions
    Task<Result<List<Transition>>> GetTransitionsAsync(string issueKeyOrId, CancellationToken cancellationToken = default);
    Task<Result> TransitionIssueAsync(string issueKeyOrId, string transitionId, CancellationToken cancellationToken = default);

    // Comments
    Task<Result<List<Comment>>> GetCommentsAsync(string issueKeyOrId, CancellationToken cancellationToken = default);
    Task<Result<Comment>> AddCommentAsync(string issueKeyOrId, string body, CancellationToken cancellationToken = default);

    // Labels
    Task<Result> AddLabelAsync(string issueKeyOrId, string label, CancellationToken cancellationToken = default);
    Task<Result> RemoveLabelAsync(string issueKeyOrId, string label, CancellationToken cancellationToken = default);

    // Assignment
    Task<Result> AssignIssueAsync(string issueKeyOrId, string? accountId, CancellationToken cancellationToken = default);

    // Issue Links
    Task<Result> LinkIssuesAsync(string inwardIssueKey, string outwardIssueKey, string linkTypeName, CancellationToken cancellationToken = default);

    // Boards
    Task<Result<List<Board>>> GetBoardsAsync(CancellationToken cancellationToken = default);
    Task<Result<Board>> GetBoardAsync(int boardId, CancellationToken cancellationToken = default);

    // Sprints
    Task<Result<List<Sprint>>> GetSprintsAsync(int boardId, CancellationToken cancellationToken = default);
    Task<Result> MoveIssuesToSprintAsync(int sprintId, List<string> issueKeys, CancellationToken cancellationToken = default);
}
