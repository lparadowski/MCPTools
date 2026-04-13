using System.ComponentModel;
using System.Net.Http.Json;
using ModelContextProtocol.Server;

namespace McpServer.Tools;

[McpServerToolType]
public static class JiraTools
{
    // Projects

    [McpServerTool(Name = "list_jira_projects")]
    [Description("List all Jira projects for the authenticated user.")]
    public static async Task<string> ListProjects(IHttpClientFactory httpFactory)
    {
        var http = httpFactory.CreateClient("JiraApi");
        var response = await http.GetAsync("/api/v1/projects");
        return await response.Content.ReadAsStringAsync();
    }

    [McpServerTool(Name = "get_jira_project")]
    [Description("Get a Jira project by key or ID.")]
    public static async Task<string> GetProject(
        IHttpClientFactory httpFactory,
        [Description("The project key (e.g. 'PROJ') or numeric ID")] string projectKeyOrId)
    {
        var http = httpFactory.CreateClient("JiraApi");
        var response = await http.GetAsync($"/api/v1/projects/{projectKeyOrId}");
        return await response.Content.ReadAsStringAsync();
    }

    // Issues

    [McpServerTool(Name = "create_jira_issue")]
    [Description("Create a new Jira issue (Epic, Story, Task, Bug, Subtask). Set parentKey to create child issues under an Epic or Story.")]
    public static async Task<string> CreateIssue(
        IHttpClientFactory httpFactory,
        [Description("The project key (e.g. 'PROJ')")] string projectKey,
        [Description("Issue type: Epic, Story, Task, Bug, or Subtask")] string issueType,
        [Description("Issue summary/title")] string summary,
        [Description("Issue description (plain text)")] string? description = null,
        [Description("Optional Jira custom text fields keyed by Jira field id, e.g. {\"customfield_10039\":\"Story text\"}")] Dictionary<string, string?>? customFields = null,
        [Description("Parent issue key for hierarchy (e.g. 'PROJ-1' for Epic parent)")] string? parentKey = null,
        [Description("Comma-separated labels to add")] string? labels = null)
    {
        var http = httpFactory.CreateClient("JiraApi");
        var labelList = labels?.Split(',', StringSplitOptions.RemoveEmptyEntries | StringSplitOptions.TrimEntries).ToList();
        var response = await http.PostAsJsonAsync("/api/v1/issues", new { projectKey, issueType, summary, description, customFields, parentKey, labels = labelList });
        return await response.Content.ReadAsStringAsync();
    }

    [McpServerTool(Name = "get_jira_issue")]
    [Description("Get a Jira issue by key or ID, including status, assignee, labels, and parent. Large descriptions are returned in chunks — use offset/maxLength to paginate. If hasMore is true, call again with the nextOffset value.")]
    public static async Task<string> GetIssue(
        IHttpClientFactory httpFactory,
        [Description("The issue key (e.g. 'PROJ-123') or numeric ID")] string issueKeyOrId,
        [Description("Character offset into the description content. Default: 0")] int offset = 0,
        [Description("Max characters to return in the description. 0 = use server-configured default")] int maxLength = 0)
    {
        var http = httpFactory.CreateClient("JiraApi");
        var response = await http.GetAsync($"/api/v1/issues/{issueKeyOrId}?offset={offset}&maxLength={maxLength}");
        return await response.Content.ReadAsStringAsync();
    }

    [McpServerTool(Name = "update_jira_issue")]
    [Description("Update a Jira issue's summary and/or description.")]
    public static async Task<string> UpdateIssue(
        IHttpClientFactory httpFactory,
        [Description("The issue key (e.g. 'PROJ-123') or numeric ID")] string issueKeyOrId,
        [Description("New summary/title (null to keep current)")] string? summary = null,
        [Description("New description (null to keep current)")] string? description = null,
        [Description("Optional Jira custom text fields keyed by Jira field id, e.g. {\"customfield_10039\":\"Story text\"}")] Dictionary<string, string?>? customFields = null)
    {
        var http = httpFactory.CreateClient("JiraApi");
        var response = await http.PutAsJsonAsync($"/api/v1/issues/{issueKeyOrId}", new { summary, description, customFields });
        return await response.Content.ReadAsStringAsync();
    }

    [McpServerTool(Name = "delete_jira_issue")]
    [Description("Permanently delete a Jira issue. This cannot be undone.")]
    public static async Task<string> DeleteIssue(
        IHttpClientFactory httpFactory,
        [Description("The issue key (e.g. 'PROJ-123') or numeric ID")] string issueKeyOrId)
    {
        var http = httpFactory.CreateClient("JiraApi");
        var response = await http.DeleteAsync($"/api/v1/issues/{issueKeyOrId}");
        return await response.Content.ReadAsStringAsync();
    }

    [McpServerTool(Name = "search_jira_issues")]
    [Description("Search Jira issues using JQL (Jira Query Language). Example: 'project = PROJ AND status = \"To Do\"'. Large results are returned in chunks — use offset/maxLength to paginate. If hasMore is true, call again with the nextOffset value.")]
    public static async Task<string> SearchIssues(
        IHttpClientFactory httpFactory,
        [Description("JQL query string")] string jql,
        [Description("Maximum results to return (default 50)")] int maxResults = 50,
        [Description("Character offset into the serialized results. Default: 0")] int offset = 0,
        [Description("Max characters to return. 0 = use server-configured default")] int maxLength = 0)
    {
        var http = httpFactory.CreateClient("JiraApi");
        var response = await http.PostAsJsonAsync($"/api/v1/issues/search?offset={offset}&maxLength={maxLength}", new { jql, maxResults });
        return await response.Content.ReadAsStringAsync();
    }

    // Transitions

    [McpServerTool(Name = "get_jira_transitions")]
    [Description("Get available workflow transitions for a Jira issue (e.g. To Do -> In Progress -> Done).")]
    public static async Task<string> GetTransitions(
        IHttpClientFactory httpFactory,
        [Description("The issue key (e.g. 'PROJ-123')")] string issueKeyOrId)
    {
        var http = httpFactory.CreateClient("JiraApi");
        var response = await http.GetAsync($"/api/v1/issues/{issueKeyOrId}/transitions");
        return await response.Content.ReadAsStringAsync();
    }

    [McpServerTool(Name = "transition_jira_issue")]
    [Description("Move a Jira issue through its workflow (e.g. To Do -> In Progress). Use get_jira_transitions first to find the transition ID.")]
    public static async Task<string> TransitionIssue(
        IHttpClientFactory httpFactory,
        [Description("The issue key (e.g. 'PROJ-123')")] string issueKeyOrId,
        [Description("The transition ID (get from get_jira_transitions)")] string transitionId)
    {
        var http = httpFactory.CreateClient("JiraApi");
        var response = await http.PostAsJsonAsync($"/api/v1/issues/{issueKeyOrId}/transitions", new { transitionId });
        return await response.Content.ReadAsStringAsync();
    }

    // Comments

    [McpServerTool(Name = "get_jira_comments")]
    [Description("Get all comments on a Jira issue.")]
    public static async Task<string> GetComments(
        IHttpClientFactory httpFactory,
        [Description("The issue key (e.g. 'PROJ-123')")] string issueKeyOrId)
    {
        var http = httpFactory.CreateClient("JiraApi");
        var response = await http.GetAsync($"/api/v1/issues/{issueKeyOrId}/comments");
        return await response.Content.ReadAsStringAsync();
    }

    [McpServerTool(Name = "add_jira_comment")]
    [Description("Add a comment to a Jira issue.")]
    public static async Task<string> AddComment(
        IHttpClientFactory httpFactory,
        [Description("The issue key (e.g. 'PROJ-123')")] string issueKeyOrId,
        [Description("The comment text")] string body)
    {
        var http = httpFactory.CreateClient("JiraApi");
        var response = await http.PostAsJsonAsync($"/api/v1/issues/{issueKeyOrId}/comments", new { body });
        return await response.Content.ReadAsStringAsync();
    }

    // Labels

    [McpServerTool(Name = "add_jira_label")]
    [Description("Add a label to a Jira issue.")]
    public static async Task<string> AddLabel(
        IHttpClientFactory httpFactory,
        [Description("The issue key (e.g. 'PROJ-123')")] string issueKeyOrId,
        [Description("The label to add")] string label)
    {
        var http = httpFactory.CreateClient("JiraApi");
        var response = await http.PostAsJsonAsync($"/api/v1/issues/{issueKeyOrId}/labels", new { label });
        return await response.Content.ReadAsStringAsync();
    }

    [McpServerTool(Name = "remove_jira_label")]
    [Description("Remove a label from a Jira issue.")]
    public static async Task<string> RemoveLabel(
        IHttpClientFactory httpFactory,
        [Description("The issue key (e.g. 'PROJ-123')")] string issueKeyOrId,
        [Description("The label to remove")] string label)
    {
        var http = httpFactory.CreateClient("JiraApi");
        var response = await http.DeleteAsync($"/api/v1/issues/{issueKeyOrId}/labels/{label}");
        return await response.Content.ReadAsStringAsync();
    }

    // Assignment

    [McpServerTool(Name = "assign_jira_issue")]
    [Description("Assign a Jira issue to a user. Pass null accountId to unassign.")]
    public static async Task<string> AssignIssue(
        IHttpClientFactory httpFactory,
        [Description("The issue key (e.g. 'PROJ-123')")] string issueKeyOrId,
        [Description("The user's account ID (null to unassign)")] string? accountId = null)
    {
        var http = httpFactory.CreateClient("JiraApi");
        var response = await http.PutAsJsonAsync($"/api/v1/issues/{issueKeyOrId}/assignee", new { accountId });
        return await response.Content.ReadAsStringAsync();
    }

    // Issue Links

    [McpServerTool(Name = "link_jira_issues")]
    [Description("Link two Jira issues together (e.g. 'Blocks', 'is blocked by', 'relates to').")]
    public static async Task<string> LinkIssues(
        IHttpClientFactory httpFactory,
        [Description("The inward issue key (e.g. 'PROJ-1')")] string inwardIssueKey,
        [Description("The outward issue key (e.g. 'PROJ-2')")] string outwardIssueKey,
        [Description("Link type name (e.g. 'Blocks', 'Relates')")] string linkTypeName)
    {
        var http = httpFactory.CreateClient("JiraApi");
        var response = await http.PostAsJsonAsync("/api/v1/issuelinks", new { inwardIssueKey, outwardIssueKey, linkTypeName });
        return await response.Content.ReadAsStringAsync();
    }

    // Boards

    [McpServerTool(Name = "list_jira_boards")]
    [Description("List all Jira boards (Scrum and Kanban).")]
    public static async Task<string> ListBoards(IHttpClientFactory httpFactory)
    {
        var http = httpFactory.CreateClient("JiraApi");
        var response = await http.GetAsync("/api/v1/boards");
        return await response.Content.ReadAsStringAsync();
    }

    [McpServerTool(Name = "get_jira_board")]
    [Description("Get a Jira board by ID.")]
    public static async Task<string> GetBoard(
        IHttpClientFactory httpFactory,
        [Description("The board ID")] int boardId)
    {
        var http = httpFactory.CreateClient("JiraApi");
        var response = await http.GetAsync($"/api/v1/boards/{boardId}");
        return await response.Content.ReadAsStringAsync();
    }

    // Sprints

    [McpServerTool(Name = "list_jira_sprints")]
    [Description("List all sprints for a Jira board.")]
    public static async Task<string> ListSprints(
        IHttpClientFactory httpFactory,
        [Description("The board ID")] int boardId)
    {
        var http = httpFactory.CreateClient("JiraApi");
        var response = await http.GetAsync($"/api/v1/sprints/by-board/{boardId}");
        return await response.Content.ReadAsStringAsync();
    }

    [McpServerTool(Name = "move_issues_to_sprint")]
    [Description("Move one or more Jira issues to a sprint.")]
    public static async Task<string> MoveIssuesToSprint(
        IHttpClientFactory httpFactory,
        [Description("The sprint ID")] int sprintId,
        [Description("Comma-separated issue keys (e.g. 'PROJ-1,PROJ-2')")] string issueKeys)
    {
        var http = httpFactory.CreateClient("JiraApi");
        var keys = issueKeys.Split(',', StringSplitOptions.RemoveEmptyEntries | StringSplitOptions.TrimEntries).ToList();
        var response = await http.PostAsJsonAsync($"/api/v1/sprints/{sprintId}/issues", new { issueKeys = keys });
        return await response.Content.ReadAsStringAsync();
    }

    // Worklogs

    [McpServerTool(Name = "get_jira_worklogs")]
    [Description("Get all worklog entries for a Jira issue (time tracked).")]
    public static async Task<string> GetWorklogs(
        IHttpClientFactory httpFactory,
        [Description("The issue key (e.g. 'PROJ-123')")] string issueKeyOrId)
    {
        var http = httpFactory.CreateClient("JiraApi");
        var response = await http.GetAsync($"/api/v1/issues/{issueKeyOrId}/worklogs");
        return await response.Content.ReadAsStringAsync();
    }

    [McpServerTool(Name = "search_jira_user_worklogs")]
    [Description("Search for all worklogs logged by a specific user within a date range. Returns worklogs with issue keys, time spent, and dates.")]
    public static async Task<string> SearchUserWorklogs(
        IHttpClientFactory httpFactory,
        [Description("The Jira username or account ID of the user")] string username,
        [Description("Start date (ISO 8601 format, e.g. '2025-03-01')")] DateTime startDate,
        [Description("End date (ISO 8601 format, e.g. '2025-03-31')")] DateTime endDate)
    {
        var http = httpFactory.CreateClient("JiraApi");
        var response = await http.GetAsync($"/api/v1/worklogs?username={Uri.EscapeDataString(username)}&startDate={startDate:yyyy-MM-dd}&endDate={endDate:yyyy-MM-dd}");
        return await response.Content.ReadAsStringAsync();
    }

    [McpServerTool(Name = "log_jira_work")]
    [Description("Log time spent on a Jira issue. Time format examples: '2h', '3d', '1h 30m', '4h 30m'.")]
    public static async Task<string> LogWork(
        IHttpClientFactory httpFactory,
        [Description("The issue key (e.g. 'PROJ-123')")] string issueKeyOrId,
        [Description("Time spent (e.g. '2h', '3d', '1h 30m')")] string timeSpent,
        [Description("Optional comment describing the work done")] string? comment = null,
        [Description("When the work was started (ISO 8601 with colon in offset, e.g. '2025-03-15T09:00:00+08:00'. Defaults to now)")] DateTime? started = null)
    {
        var http = httpFactory.CreateClient("JiraApi");
        var response = await http.PostAsJsonAsync($"/api/v1/issues/{issueKeyOrId}/worklogs", new { timeSpent, comment, started });
        return await response.Content.ReadAsStringAsync();
    }

    [McpServerTool(Name = "update_jira_worklog")]
    [Description("Update an existing worklog entry on a Jira issue. Use search_jira_user_worklogs to find worklog IDs.")]
    public static async Task<string> UpdateWorklog(
        IHttpClientFactory httpFactory,
        [Description("The issue key (e.g. 'PROJ-123')")] string issueKeyOrId,
        [Description("The worklog ID to update")] string worklogId,
        [Description("Time spent (e.g. '2h', '3d', '1h 30m')")] string timeSpent,
        [Description("Optional comment describing the work done")] string? comment = null,
        [Description("When the work was started (ISO 8601 with colon in offset, e.g. '2025-03-15T09:00:00+08:00')")] DateTime? started = null)
    {
        var http = httpFactory.CreateClient("JiraApi");
        var response = await http.PutAsJsonAsync($"/api/v1/worklogs/{issueKeyOrId}/{worklogId}", new { timeSpent, comment, started });
        return await response.Content.ReadAsStringAsync();
    }

    [McpServerTool(Name = "delete_jira_worklog")]
    [Description("Delete a worklog entry from a Jira issue. Use search_jira_user_worklogs to find worklog IDs.")]
    public static async Task<string> DeleteWorklog(
        IHttpClientFactory httpFactory,
        [Description("The issue key (e.g. 'PROJ-123')")] string issueKeyOrId,
        [Description("The worklog ID to delete")] string worklogId)
    {
        var http = httpFactory.CreateClient("JiraApi");
        var response = await http.DeleteAsync($"/api/v1/worklogs/{issueKeyOrId}/{worklogId}");
        return await response.Content.ReadAsStringAsync();
    }

    // Activity

    [McpServerTool(Name = "get_jira_user_activity")]
    [Description("Get a user's Jira activity for a date range: tickets assigned, status transitions, field changes, and comments made. Useful for understanding what someone worked on each day. Large results are returned in chunks — use offset/maxLength to paginate. If hasMore is true, call again with the nextOffset value.")]
    public static async Task<string> GetUserActivity(
        IHttpClientFactory httpFactory,
        [Description("The user's Jira account ID")] string accountId,
        [Description("Start date (ISO 8601 format, e.g. '2025-03-01')")] DateTime startDate,
        [Description("End date (ISO 8601 format, e.g. '2025-03-31')")] DateTime endDate,
        [Description("Character offset into the serialized results. Default: 0")] int offset = 0,
        [Description("Max characters to return. 0 = use server-configured default")] int maxLength = 0)
    {
        var http = httpFactory.CreateClient("JiraApi");
        var response = await http.GetAsync($"/api/v1/activity?accountId={Uri.EscapeDataString(accountId)}&startDate={startDate:yyyy-MM-dd}&endDate={endDate:yyyy-MM-dd}&offset={offset}&maxLength={maxLength}");
        return await response.Content.ReadAsStringAsync();
    }

    // Fields

    [McpServerTool(Name = "list_jira_fields")]
    [Description("List all Jira fields with their IDs and display names. Use this to find custom field IDs (e.g. customfield_XXXXX) for fields like 'Acceptance Criteria', 'Story Points', etc.")]
    public static async Task<string> ListFields(IHttpClientFactory httpFactory)
    {
        var http = httpFactory.CreateClient("JiraApi");
        var response = await http.GetAsync("/api/v1/fields");
        return await response.Content.ReadAsStringAsync();
    }
}
