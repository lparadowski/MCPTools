using System.ComponentModel;
using System.Net.Http.Json;
using ModelContextProtocol.Server;

namespace McpServer.Tools;

[McpServerToolType]
public static class PolarionTools
{
    // Projects

    [McpServerTool(Name = "list_polarion_projects")]
    [Description("List all Polarion projects.")]
    public static async Task<string> ListProjects(IHttpClientFactory httpFactory)
    {
        var http = httpFactory.CreateClient("PolarionApi");
        var response = await http.GetAsync("/api/v1/projects");
        return await response.ReadContentOrError();
    }

    [McpServerTool(Name = "get_polarion_project")]
    [Description("Get a Polarion project by ID.")]
    public static async Task<string> GetProject(
        IHttpClientFactory httpFactory,
        [Description("The project ID")] string projectId)
    {
        var http = httpFactory.CreateClient("PolarionApi");
        var response = await http.GetAsync($"/api/v1/projects/{projectId}");
        return await response.ReadContentOrError();
    }

    // Requirements

    [McpServerTool(Name = "list_polarion_requirements")]
    [Description("List requirements (work items) in a Polarion project. Optionally filter with a Polarion query string.")]
    public static async Task<string> ListRequirements(
        IHttpClientFactory httpFactory,
        [Description("The project ID")] string projectId,
        [Description("Polarion query to filter requirements (e.g. 'type:requirement AND status:approved')")] string? query = null,
        [Description("Maximum results to return (default 50)")] int maxResults = 50)
    {
        var http = httpFactory.CreateClient("PolarionApi");
        var url = $"/api/v1/projects/{projectId}/requirements?maxResults={maxResults}";
        if (!string.IsNullOrWhiteSpace(query))
        {
            url += $"&query={Uri.EscapeDataString(query)}";
        }
        var response = await http.GetAsync(url);
        return await response.ReadContentOrError();
    }

    [McpServerTool(Name = "get_polarion_requirement")]
    [Description("Get a specific requirement (work item) from a Polarion project by its ID.")]
    public static async Task<string> GetRequirement(
        IHttpClientFactory httpFactory,
        [Description("The project ID")] string projectId,
        [Description("The work item ID")] string workItemId)
    {
        var http = httpFactory.CreateClient("PolarionApi");
        var response = await http.GetAsync($"/api/v1/projects/{projectId}/requirements/{workItemId}");
        return await response.ReadContentOrError();
    }

    [McpServerTool(Name = "get_polarion_requirement_links")]
    [Description("Get linked work items for a requirement, showing traceability (parent/child, related, verifies, etc.).")]
    public static async Task<string> GetRequirementLinks(
        IHttpClientFactory httpFactory,
        [Description("The project ID")] string projectId,
        [Description("The work item ID")] string workItemId)
    {
        var http = httpFactory.CreateClient("PolarionApi");
        var response = await http.GetAsync($"/api/v1/projects/{projectId}/requirements/{workItemId}/links");
        return await response.ReadContentOrError();
    }
}
