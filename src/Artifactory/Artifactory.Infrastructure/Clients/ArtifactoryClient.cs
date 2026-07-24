using System.Net;
using System.Net.Http.Json;
using Artifactory.Application.Interfaces;
using Artifactory.Domain.Entities;
using Artifactory.Infrastructure.Dtos;

namespace Artifactory.Infrastructure.Clients;

public class ArtifactoryClient(IHttpClientFactory httpClientFactory) : IArtifactoryClient
{
    // Cap how many matches we enrich with properties so a broad name match doesn't
    // fan out into hundreds of per-item storage calls.
    private const int MaxItems = 25;

    // Cap how many produced artifacts we surface per build.
    private const int MaxArtifacts = 50;

    public async Task<List<ArtifactItem>> SearchArtifactsAsync(
        string name, string? repo = null, CancellationToken cancellationToken = default)
    {
        var http = httpClientFactory.CreateClient("ArtifactoryApi");

        var url = $"/artifactory/api/search/artifact?name={Uri.EscapeDataString(name)}";
        if (!string.IsNullOrWhiteSpace(repo))
        {
            url += $"&repos={Uri.EscapeDataString(repo)}";
        }

        var search = await http.GetFromJsonAsync<ArtifactSearchResultDto>(url, cancellationToken);
        var hits = (search?.Results ?? []).Where(r => !string.IsNullOrWhiteSpace(r.Uri)).Take(MaxItems);

        var items = new List<ArtifactItem>();
        foreach (var hit in hits)
        {
            var (repoKey, path) = ParseStorageUri(hit.Uri);
            var properties = await GetPropertiesAsync(http, hit.Uri, cancellationToken);

            items.Add(new ArtifactItem
            {
                Repo = repoKey,
                Path = path,
                Name = path.Length == 0 ? repoKey : path[(path.LastIndexOf('/') + 1)..],
                Properties = properties
            });
        }

        return items;
    }

    public async Task<List<BuildSummary>> GetBuildsAsync(string? filter = null, CancellationToken cancellationToken = default)
    {
        var http = httpClientFactory.CreateClient("ArtifactoryApi");

        var list = await http.GetFromJsonAsync<BuildsListDto>("/artifactory/api/build", cancellationToken);

        IEnumerable<BuildSummary> builds = (list?.Builds ?? [])
            .Select(b => new BuildSummary
            {
                Name = Uri.UnescapeDataString(b.Uri.TrimStart('/')),
                LastStarted = string.IsNullOrWhiteSpace(b.LastStarted) ? null : b.LastStarted
            });

        if (!string.IsNullOrWhiteSpace(filter))
        {
            builds = builds.Where(b => b.Name.Contains(filter, StringComparison.OrdinalIgnoreCase));
        }

        return builds.OrderBy(b => b.Name).ToList();
    }

    public async Task<BuildDetail?> GetBuildInfoAsync(string name, string? number = null, CancellationToken cancellationToken = default)
    {
        var http = httpClientFactory.CreateClient("ArtifactoryApi");
        var encodedName = Uri.EscapeDataString(name);

        if (string.IsNullOrWhiteSpace(number))
        {
            var runsResponse = await http.GetAsync($"/artifactory/api/build/{encodedName}", cancellationToken);
            if (!runsResponse.IsSuccessStatusCode)
            {
                return null;
            }

            var runs = await runsResponse.Content.ReadFromJsonAsync<BuildRunsDto>(cancellationToken: cancellationToken);

            // ISO-8601 timestamps sort chronologically as strings, so the max is the latest run.
            number = runs?.BuildsNumbers
                .OrderByDescending(r => r.Started)
                .FirstOrDefault()?.Uri.TrimStart('/');

            if (string.IsNullOrWhiteSpace(number))
            {
                return null;
            }
        }

        var response = await http.GetAsync(
            $"/artifactory/api/build/{encodedName}/{Uri.EscapeDataString(number)}", cancellationToken);
        if (!response.IsSuccessStatusCode)
        {
            return null;
        }

        var dto = await response.Content.ReadFromJsonAsync<BuildInfoDto>(cancellationToken: cancellationToken);
        var body = dto?.BuildInfo;
        if (body is null)
        {
            return null;
        }

        return MapBuildDetail(name, body);
    }

    private static BuildDetail MapBuildDetail(string name, BuildInfoBodyDto body)
    {
        var vcs = body.Vcs?.FirstOrDefault();
        var props = body.Properties ?? new Dictionary<string, string>();

        var commit = FirstNonEmpty(
            vcs?.Revision,
            FindProperty(props, "buildInfo.env.BUILD_SOURCEVERSION", "buildInfo.env.GIT_COMMIT", "buildInfo.env.CI_COMMIT_SHA"));

        var branch = FirstNonEmpty(
            vcs?.Branch,
            NormalizeBranch(FindProperty(props,
                "buildInfo.env.BUILD_SOURCEBRANCHNAME", "buildInfo.env.BUILD_SOURCEBRANCH",
                "buildInfo.env.GIT_BRANCH", "buildInfo.env.CI_COMMIT_REF_NAME")));

        var repoUrl = FirstNonEmpty(
            vcs?.Url,
            FindProperty(props, "buildInfo.env.BUILD_REPOSITORY_URI", "buildInfo.env.BUILD_REPOSITORY_NAME"));

        var artifacts = (body.Modules ?? [])
            .SelectMany(m => m.Artifacts ?? [])
            .Select(a => a.Name)
            .Where(n => !string.IsNullOrWhiteSpace(n))
            .Select(n => n!)
            .Distinct()
            .Take(MaxArtifacts)
            .ToList();

        return new BuildDetail
        {
            Name = name,
            Number = body.Number ?? string.Empty,
            Started = body.Started,
            BuildUrl = body.Url,
            Agent = body.Agent?.Name,
            CommitSha = commit,
            Branch = branch,
            RepoUrl = repoUrl,
            Artifacts = artifacts
        };
    }

    private static string? FindProperty(Dictionary<string, string> props, params string[] keys)
    {
        foreach (var key in keys)
        {
            var match = props.FirstOrDefault(p => string.Equals(p.Key, key, StringComparison.OrdinalIgnoreCase));
            if (!string.IsNullOrWhiteSpace(match.Value))
            {
                return match.Value;
            }
        }

        return null;
    }

    private static string? NormalizeBranch(string? branch) =>
        branch?.Replace("refs/heads/", string.Empty, StringComparison.OrdinalIgnoreCase);

    private static string? FirstNonEmpty(params string?[] values) =>
        values.FirstOrDefault(v => !string.IsNullOrWhiteSpace(v));

    private static async Task<Dictionary<string, List<string>>> GetPropertiesAsync(
        HttpClient http, string storageUri, CancellationToken cancellationToken)
    {
        // Use only the path of the returned URI so we stay on the configured host,
        // regardless of what hostname Artifactory echoes back in the search results.
        var path = new Uri(storageUri).PathAndQuery;
        var response = await http.GetAsync($"{path}?properties", cancellationToken);

        // Artifactory returns 404 when an item simply has no properties — that's an empty result, not a failure.
        if (response.StatusCode == HttpStatusCode.NotFound || !response.IsSuccessStatusCode)
        {
            return [];
        }

        var dto = await response.Content.ReadFromJsonAsync<ArtifactPropertiesDto>(cancellationToken: cancellationToken);
        return dto?.Properties ?? [];
    }

    private static (string Repo, string Path) ParseStorageUri(string storageUri)
    {
        // Storage URIs look like {base}/api/storage/{repo}/{path...}.
        const string marker = "/api/storage/";
        var absolutePath = new Uri(storageUri).AbsolutePath;
        var index = absolutePath.IndexOf(marker, StringComparison.OrdinalIgnoreCase);
        if (index < 0)
        {
            return (string.Empty, string.Empty);
        }

        var remainder = absolutePath[(index + marker.Length)..].Trim('/');
        var slash = remainder.IndexOf('/');
        return slash < 0
            ? (remainder, string.Empty)
            : (remainder[..slash], remainder[(slash + 1)..]);
    }
}
