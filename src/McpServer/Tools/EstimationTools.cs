using System.ComponentModel;
using System.Text.Json;
using ModelContextProtocol.Server;

namespace McpServer.Tools;

[McpServerToolType]
public static class EstimationTools
{
    [McpServerTool(Name = "get_ticket_profile")]
    [Description("Get a comprehensive profile for a Jira ticket: status durations from changelog, worklogs, and associated GitHub PRs with code stats. Use the teams config (get_digest_config) to interpret which statuses are active vs unreliable.")]
    public static async Task<string> GetTicketProfile(
        IHttpClientFactory httpFactory,
        [Description("The Jira issue key")] string jiraKey,
        [Description("GitHub org to scope PR search (e.g. 'fmgl-autonomy'). Without this, search may match unrelated repos.")] string? githubOrg = null)
    {
        var jiraHttp = httpFactory.CreateClient("JiraApi");
        var githubHttp = httpFactory.CreateClient("GitHubApi");

        var jiraTask = jiraHttp.GetAsync($"/api/v1/issues/{jiraKey}/profile");

        var prQuery = githubOrg is not null ? $"{jiraKey} org:{githubOrg}" : jiraKey;
        var githubTask = githubHttp.GetAsync($"/api/v1/search/pull-requests?query={Uri.EscapeDataString(prQuery)}");

        await Task.WhenAll(jiraTask, githubTask);

        var jiraProfile = await jiraTask.Result.Content.ReadAsStringAsync();
        var githubPrs = "[]";

        if (githubTask.Result.IsSuccessStatusCode)
        {
            githubPrs = await githubTask.Result.Content.ReadAsStringAsync();
        }

        if (!jiraTask.Result.IsSuccessStatusCode)
        {
            return await jiraTask.Result.ReadContentOrError();
        }

        var result = new
        {
            jira = JsonSerializer.Deserialize<JsonElement>(jiraProfile),
            github = new
            {
                pullRequests = JsonSerializer.Deserialize<JsonElement>(githubPrs)
            }
        };

        return JsonSerializer.Serialize(result, new JsonSerializerOptions { WriteIndented = true });
    }
}
