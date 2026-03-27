using Jira.Domain.Entities;

namespace Jira.Application.Interfaces;

public interface IJiraClient
{
    // Projects
    Task<List<Project>> GetProjectsAsync(CancellationToken cancellationToken = default);
    Task<Project?> GetProjectAsync(string projectKeyOrId, CancellationToken cancellationToken = default);

    // Issues
    Task<Issue?> CreateIssueAsync(
        string projectKey,
        string issueType,
        string summary,
        string? description,
        Dictionary<string, string?>? customFields,
        string? parentKey,
        List<string>? labels,
        CancellationToken cancellationToken = default);
    Task<Issue?> GetIssueAsync(string issueKeyOrId, CancellationToken cancellationToken = default);
    Task<Issue?> UpdateIssueAsync(
        string issueKeyOrId,
        string? summary,
        string? description,
        Dictionary<string, string?>? customFields,
        CancellationToken cancellationToken = default);
    Task<bool> DeleteIssueAsync(string issueKeyOrId, CancellationToken cancellationToken = default);
    Task<List<Issue>> SearchIssuesAsync(string jql, int maxResults = 50, CancellationToken cancellationToken = default);

    // Transitions
    Task<List<Transition>> GetTransitionsAsync(string issueKeyOrId, CancellationToken cancellationToken = default);
    Task<bool> TransitionIssueAsync(string issueKeyOrId, string transitionId, CancellationToken cancellationToken = default);

    // Comments
    Task<List<Comment>> GetCommentsAsync(string issueKeyOrId, CancellationToken cancellationToken = default);
    Task<Comment?> AddCommentAsync(string issueKeyOrId, string body, CancellationToken cancellationToken = default);

    // Labels
    Task<bool> AddLabelAsync(string issueKeyOrId, string label, CancellationToken cancellationToken = default);
    Task<bool> RemoveLabelAsync(string issueKeyOrId, string label, CancellationToken cancellationToken = default);

    // Assignment
    Task<bool> AssignIssueAsync(string issueKeyOrId, string? accountId, CancellationToken cancellationToken = default);

    // Issue Links
    Task<bool> LinkIssuesAsync(string inwardIssueKey, string outwardIssueKey, string linkTypeName, CancellationToken cancellationToken = default);

    // Boards
    Task<List<Board>> GetBoardsAsync(CancellationToken cancellationToken = default);
    Task<Board?> GetBoardAsync(int boardId, CancellationToken cancellationToken = default);

    // Sprints
    Task<List<Sprint>> GetSprintsAsync(int boardId, CancellationToken cancellationToken = default);
    Task<bool> MoveIssuesToSprintAsync(int sprintId, List<string> issueKeys, CancellationToken cancellationToken = default);

    // Worklogs
    Task<List<Worklog>> GetWorklogsAsync(string issueKeyOrId, CancellationToken cancellationToken = default);
    Task<Worklog?> AddWorklogAsync(string issueKeyOrId, string timeSpent, string? comment, DateTime? started, CancellationToken cancellationToken = default);
    Task<List<Worklog>> GetUserWorklogsAsync(string username, DateTime startDate, DateTime endDate, CancellationToken cancellationToken = default);
    Task<Worklog?> UpdateWorklogAsync(string issueKeyOrId, string worklogId, string timeSpent, string? comment, DateTime? started, CancellationToken cancellationToken = default);
    Task<bool> DeleteWorklogAsync(string issueKeyOrId, string worklogId, CancellationToken cancellationToken = default);

    // Activity
    Task<List<UserActivity>> GetUserActivityAsync(string accountId, DateTime startDate, DateTime endDate, CancellationToken cancellationToken = default);

    // Fields
    Task<Dictionary<string, string>> GetFieldsAsync(CancellationToken cancellationToken = default);
}
