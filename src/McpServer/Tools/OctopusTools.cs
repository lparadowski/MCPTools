using System.ComponentModel;
using ModelContextProtocol.Server;

namespace McpServer.Tools;

[McpServerToolType]
public static class OctopusTools
{
    [McpServerTool(Name = "list_octopus_environments")]
    [Description("List all environments in Octopus Deploy with their ids and exact names. Use this to find the exact environment name to pass to get_octopus_deployed_version.")]
    public static async Task<string> ListEnvironments(IHttpClientFactory httpFactory)
    {
        var http = httpFactory.CreateClient("OctopusApi");
        var response = await http.GetAsync("/api/v1/environments");
        return await response.ReadContentOrError();
    }

    [McpServerTool(Name = "list_octopus_projects")]
    [Description("List all projects (applications) in Octopus Deploy with their ids and exact names. Use this to find the exact project name to pass to get_octopus_deployed_version.")]
    public static async Task<string> ListProjects(IHttpClientFactory httpFactory)
    {
        var http = httpFactory.CreateClient("OctopusApi");
        var response = await http.GetAsync("/api/v1/projects");
        return await response.ReadContentOrError();
    }

    [McpServerTool(Name = "get_octopus_deployed_version")]
    [Description("Find which release/build version was live on an Octopus Deploy environment at a point in time. " +
                 "Returns the most recent successful deployment to that environment on or before the given time — " +
                 "for one project if specified, otherwise for every project deployed to that environment. " +
                 "Note: the deploy time reflects when the deployment was queued/started.")]
    public static async Task<string> GetDeployedVersion(
        IHttpClientFactory httpFactory,
        [Description("The Octopus environment name, exactly as it appears in Octopus (e.g. 'Production', 'Staging')")] string environment,
        [Description("Point in time as ISO-8601 (e.g. '2026-07-15T10:00:00Z') or a date 'yyyy-MM-dd'. Interpreted as UTC when no offset is given. Defaults to now.")] string? asOf = null,
        [Description("Optional Octopus project name to narrow to a single application. Omit to get every project's live version on the environment.")] string? project = null)
    {
        var http = httpFactory.CreateClient("OctopusApi");

        var query = new List<string> { $"environment={Uri.EscapeDataString(environment)}" };
        if (!string.IsNullOrWhiteSpace(asOf))
        {
            query.Add($"asOf={Uri.EscapeDataString(asOf)}");
        }

        if (!string.IsNullOrWhiteSpace(project))
        {
            query.Add($"project={Uri.EscapeDataString(project)}");
        }

        var response = await http.GetAsync($"/api/v1/deployments/deployed?{string.Join("&", query)}");
        return await response.ReadContentOrError();
    }
}
