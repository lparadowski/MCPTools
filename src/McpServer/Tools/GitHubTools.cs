using System.ComponentModel;
using System.Net.Http.Json;
using ModelContextProtocol.Server;

namespace McpServer.Tools;

[McpServerToolType]
public static class GitHubTools
{
    [McpServerTool(Name = "list_github_repositories")]
    [Description("List repositories for the authenticated GitHub user.")]
    public static async Task<string> ListRepositories(IHttpClientFactory httpFactory)
    {
        var http = httpFactory.CreateClient("GitHubApi");
        var response = await http.GetAsync("/api/v1/repositories");
        return await response.ReadContentOrError();
    }

    [McpServerTool(Name = "get_github_repository")]
    [Description("Get a GitHub repository by owner and name.")]
    public static async Task<string> GetRepository(
        IHttpClientFactory httpFactory,
        [Description("The repository owner (user or organization)")] string owner,
        [Description("The repository name")] string repo)
    {
        var http = httpFactory.CreateClient("GitHubApi");
        var response = await http.GetAsync($"/api/v1/repositories/{owner}/{repo}");
        return await response.ReadContentOrError();
    }

    [McpServerTool(Name = "get_github_user_activity")]
    [Description("Get a GitHub user's activity (pushes, PRs, comments, reviews, etc.) for a date or date range. Defaults to the authenticated user and today's date if not specified.")]
    public static async Task<string> GetUserActivity(
        IHttpClientFactory httpFactory,
        [Description("GitHub username to look up (defaults to authenticated user)")] string? username = null,
        [Description("Start date in yyyy-MM-dd format (defaults to today)")] string? from = null,
        [Description("End date in yyyy-MM-dd format (defaults to same as from)")] string? to = null)
    {
        var http = httpFactory.CreateClient("GitHubApi");
        var url = "/api/v1/activity";
        var queryParams = new List<string>();

        if (!string.IsNullOrWhiteSpace(username))
            queryParams.Add($"username={username}");
        if (!string.IsNullOrWhiteSpace(from))
            queryParams.Add($"from={from}");
        if (!string.IsNullOrWhiteSpace(to))
            queryParams.Add($"to={to}");

        if (queryParams.Count > 0)
            url += "?" + string.Join("&", queryParams);

        var response = await http.GetAsync(url);
        return await response.ReadContentOrError();
    }

    // Pull Requests

    [McpServerTool(Name = "list_github_pull_requests")]
    [Description("List pull requests for a GitHub repository. Filter by state: open, closed, or all.")]
    public static async Task<string> ListPullRequests(
        IHttpClientFactory httpFactory,
        [Description("The repository owner (user or organization)")] string owner,
        [Description("The repository name")] string repo,
        [Description("PR state filter: open, closed, or all (default: open)")] string? state = null,
        [Description("Maximum results to return (default 30)")] int maxResults = 30)
    {
        var http = httpFactory.CreateClient("GitHubApi");
        var url = $"/api/v1/repositories/{owner}/{repo}/pullrequests?maxResults={maxResults}";
        if (!string.IsNullOrWhiteSpace(state))
            url += $"&state={state}";
        var response = await http.GetAsync(url);
        return await response.ReadContentOrError();
    }

    [McpServerTool(Name = "get_github_pull_request")]
    [Description("Get a specific pull request from a GitHub repository by number.")]
    public static async Task<string> GetPullRequest(
        IHttpClientFactory httpFactory,
        [Description("The repository owner (user or organization)")] string owner,
        [Description("The repository name")] string repo,
        [Description("The pull request number")] int number)
    {
        var http = httpFactory.CreateClient("GitHubApi");
        var response = await http.GetAsync($"/api/v1/repositories/{owner}/{repo}/pullrequests/{number}");
        return await response.ReadContentOrError();
    }
}
