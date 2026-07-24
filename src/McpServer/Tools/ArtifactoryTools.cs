using System.ComponentModel;
using ModelContextProtocol.Server;

namespace McpServer.Tools;

[McpServerToolType]
public static class ArtifactoryTools
{
    [McpServerTool(Name = "search_artifactory_artifact")]
    [Description("Search JFrog Artifactory for artifacts by file name and return each match with its properties. " +
                 "Artifact properties are where build and VCS provenance live — look for keys like 'build.name', " +
                 "'build.number', 'vcs.revision' (the git commit SHA) and 'vcs.url' (the source repo). " +
                 "Use this to trace a deployed package back to the commit that produced it. " +
                 "Note: which properties exist depends on what the build pipeline published to Artifactory.")]
    public static async Task<string> SearchArtifact(
        IHttpClientFactory httpFactory,
        [Description("Artifact file name to search for. Supports '*' and '?' wildcards, e.g. 'fms-swarm*1.8.0*' or an exact file name.")] string name,
        [Description("Optional Artifactory repository key to limit the search to a single repo.")] string? repo = null)
    {
        var http = httpFactory.CreateClient("ArtifactoryApi");

        var query = $"name={Uri.EscapeDataString(name)}";
        if (!string.IsNullOrWhiteSpace(repo))
        {
            query += $"&repo={Uri.EscapeDataString(repo)}";
        }

        var response = await http.GetAsync($"/api/v1/artifacts/search?{query}");
        return await response.ReadContentOrError();
    }

    [McpServerTool(Name = "list_artifactory_builds")]
    [Description("List CI build names known to JFrog Artifactory (build-info), each with the time it last ran. " +
                 "Optionally filter to builds whose name contains a substring. Use this to discover the exact " +
                 "build name to pass to get_artifactory_build_info.")]
    public static async Task<string> ListBuilds(
        IHttpClientFactory httpFactory,
        [Description("Optional case-insensitive substring to filter build names, e.g. 'fms' or 'swarm'.")] string? filter = null)
    {
        var http = httpFactory.CreateClient("ArtifactoryApi");

        var path = "/api/v1/builds";
        if (!string.IsNullOrWhiteSpace(filter))
        {
            path += $"?filter={Uri.EscapeDataString(filter)}";
        }

        var response = await http.GetAsync(path);
        return await response.ReadContentOrError();
    }

    [McpServerTool(Name = "get_artifactory_build_info")]
    [Description("Get the build-info for a JFrog Artifactory build: when it ran, the CI build-run URL, the " +
                 "artifacts it produced, and a best-effort source commit SHA and branch (read from the build's " +
                 "VCS metadata or well-known CI env vars). Use this to trace a build back to the commit that " +
                 "produced it, then resolve that commit to a pull request with the GitHub tools. " +
                 "Defaults to the latest run when no build number is given.")]
    public static async Task<string> GetBuildInfo(
        IHttpClientFactory httpFactory,
        [Description("Exact build name as listed by list_artifactory_builds.")] string name,
        [Description("Optional build number/run to fetch. Omit to get the most recent run.")] string? number = null)
    {
        var http = httpFactory.CreateClient("ArtifactoryApi");

        var query = $"name={Uri.EscapeDataString(name)}";
        if (!string.IsNullOrWhiteSpace(number))
        {
            query += $"&number={Uri.EscapeDataString(number)}";
        }

        var response = await http.GetAsync($"/api/v1/builds/info?{query}");
        return await response.ReadContentOrError();
    }
}
