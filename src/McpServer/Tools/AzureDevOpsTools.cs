using System.ComponentModel;
using System.Net.Http.Json;
using ModelContextProtocol.Server;

namespace McpServer.Tools;

[McpServerToolType]
public static class AzureDevOpsTools
{
    // Projects

    [McpServerTool(Name = "list_azure_projects")]
    [Description("List all Azure DevOps projects for the configured organization.")]
    public static async Task<string> ListProjects(IHttpClientFactory httpFactory)
    {
        var http = httpFactory.CreateClient("AzureDevOpsApi");
        var response = await http.GetAsync("/api/v1/projects");
        return await response.ReadContentOrError();
    }

    [McpServerTool(Name = "get_azure_project")]
    [Description("Get an Azure DevOps project by name or ID.")]
    public static async Task<string> GetProject(
        IHttpClientFactory httpFactory,
        [Description("The project name or ID")] string projectNameOrId)
    {
        var http = httpFactory.CreateClient("AzureDevOpsApi");
        var response = await http.GetAsync($"/api/v1/projects/{projectNameOrId}");
        return await response.ReadContentOrError();
    }

    // Work Items

    [McpServerTool(Name = "get_azure_work_item")]
    [Description("Get an Azure DevOps work item by ID, including title, state, type, assignee, tags, and parent.")]
    public static async Task<string> GetWorkItem(
        IHttpClientFactory httpFactory,
        [Description("The work item ID")] int id)
    {
        var http = httpFactory.CreateClient("AzureDevOpsApi");
        var response = await http.GetAsync($"/api/v1/workitems/{id}");
        return await response.ReadContentOrError();
    }

    [McpServerTool(Name = "create_azure_work_item")]
    [Description("Create a new Azure DevOps work item (Epic, Feature, User Story, Task, Bug).")]
    public static async Task<string> CreateWorkItem(
        IHttpClientFactory httpFactory,
        [Description("The project name")] string project,
        [Description("Work item type: Epic, Feature, User Story, Task, or Bug")] string workItemType,
        [Description("Work item title")] string title,
        [Description("Work item description (HTML supported)")] string? description = null,
        [Description("Parent work item ID for hierarchy")] int? parentId = null,
        [Description("Display name or email of the assignee")] string? assignedTo = null,
        [Description("Area path (e.g. 'Project\\Team')")] string? areaPath = null,
        [Description("Iteration path (e.g. 'Project\\Sprint 1')")] string? iterationPath = null,
        [Description("Priority (1=Critical, 2=High, 3=Medium, 4=Low)")] string? priority = null,
        [Description("Semicolon-separated tags")] string? tags = null)
    {
        var http = httpFactory.CreateClient("AzureDevOpsApi");
        var response = await http.PostAsJsonAsync("/api/v1/workitems",
            new { project, workItemType, title, description, parentId, assignedTo, areaPath, iterationPath, priority, tags });
        return await response.ReadContentOrError();
    }

    [McpServerTool(Name = "update_azure_work_item")]
    [Description("Update an existing Azure DevOps work item's fields (title, description, state, assignee, tags, etc.).")]
    public static async Task<string> UpdateWorkItem(
        IHttpClientFactory httpFactory,
        [Description("The work item ID")] int id,
        [Description("New title (null to keep current)")] string? title = null,
        [Description("New description (null to keep current)")] string? description = null,
        [Description("New state (e.g. 'New', 'Active', 'Resolved', 'Closed')")] string? state = null,
        [Description("New assignee display name or email (null to keep current)")] string? assignedTo = null,
        [Description("New area path (null to keep current)")] string? areaPath = null,
        [Description("New iteration path (null to keep current)")] string? iterationPath = null,
        [Description("New priority 1-4 (null to keep current)")] string? priority = null,
        [Description("New tags, semicolon-separated (null to keep current)")] string? tags = null)
    {
        var http = httpFactory.CreateClient("AzureDevOpsApi");
        var response = await http.PutAsJsonAsync($"/api/v1/workitems/{id}",
            new { title, description, state, assignedTo, areaPath, iterationPath, priority, tags });
        return await response.ReadContentOrError();
    }

    [McpServerTool(Name = "delete_azure_work_item")]
    [Description("Delete an Azure DevOps work item. Moves it to the recycle bin.")]
    public static async Task<string> DeleteWorkItem(
        IHttpClientFactory httpFactory,
        [Description("The work item ID")] int id)
    {
        var http = httpFactory.CreateClient("AzureDevOpsApi");
        var response = await http.DeleteAsync($"/api/v1/workitems/{id}");
        return await response.ReadContentOrError();
    }

    [McpServerTool(Name = "search_azure_work_items")]
    [Description("Search Azure DevOps work items using WIQL (Work Item Query Language). Example: SELECT [System.Id], [System.Title] FROM WorkItems WHERE [System.TeamProject] = 'MyProject' AND [System.State] = 'Active'")]
    public static async Task<string> SearchWorkItems(
        IHttpClientFactory httpFactory,
        [Description("WIQL query string")] string wiql)
    {
        var http = httpFactory.CreateClient("AzureDevOpsApi");
        var response = await http.PostAsJsonAsync("/api/v1/workitems/search", new { wiql });
        return await response.ReadContentOrError();
    }

    // Comments

    [McpServerTool(Name = "get_azure_work_item_comments")]
    [Description("Get all comments on an Azure DevOps work item.")]
    public static async Task<string> GetComments(
        IHttpClientFactory httpFactory,
        [Description("The work item ID")] int id,
        [Description("The project name")] string project)
    {
        var http = httpFactory.CreateClient("AzureDevOpsApi");
        var response = await http.GetAsync($"/api/v1/workitems/{id}/comments?project={Uri.EscapeDataString(project)}");
        return await response.ReadContentOrError();
    }

    [McpServerTool(Name = "add_azure_work_item_comment")]
    [Description("Add a comment to an Azure DevOps work item.")]
    public static async Task<string> AddComment(
        IHttpClientFactory httpFactory,
        [Description("The work item ID")] int id,
        [Description("The project name")] string project,
        [Description("Comment text")] string text)
    {
        var http = httpFactory.CreateClient("AzureDevOpsApi");
        var response = await http.PostAsJsonAsync(
            $"/api/v1/workitems/{id}/comments?project={Uri.EscapeDataString(project)}",
            new { text });
        return await response.ReadContentOrError();
    }

    // Teams

    [McpServerTool(Name = "list_azure_teams")]
    [Description("List all teams in an Azure DevOps project.")]
    public static async Task<string> ListTeams(
        IHttpClientFactory httpFactory,
        [Description("The project name")] string project)
    {
        var http = httpFactory.CreateClient("AzureDevOpsApi");
        var response = await http.GetAsync($"/api/v1/projects/{Uri.EscapeDataString(project)}/teams");
        return await response.ReadContentOrError();
    }

    // Boards

    [McpServerTool(Name = "list_azure_boards")]
    [Description("List all boards for a team in an Azure DevOps project.")]
    public static async Task<string> ListBoards(
        IHttpClientFactory httpFactory,
        [Description("The project name")] string project,
        [Description("The team name")] string team)
    {
        var http = httpFactory.CreateClient("AzureDevOpsApi");
        var response = await http.GetAsync($"/api/v1/projects/{Uri.EscapeDataString(project)}/teams/{Uri.EscapeDataString(team)}/boards");
        return await response.ReadContentOrError();
    }

    // Sprints

    [McpServerTool(Name = "list_azure_sprints")]
    [Description("List all sprints (iterations) for a team in an Azure DevOps project.")]
    public static async Task<string> ListSprints(
        IHttpClientFactory httpFactory,
        [Description("The project name")] string project,
        [Description("The team name")] string team)
    {
        var http = httpFactory.CreateClient("AzureDevOpsApi");
        var response = await http.GetAsync($"/api/v1/projects/{Uri.EscapeDataString(project)}/teams/{Uri.EscapeDataString(team)}/sprints");
        return await response.ReadContentOrError();
    }
}
