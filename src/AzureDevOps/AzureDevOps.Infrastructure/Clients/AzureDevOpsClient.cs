using System.Net.Http.Json;
using System.Text;
using System.Text.Json;
using AzureDevOps.Application.Interfaces;
using AzureDevOps.Domain.Entities;
using AzureDevOps.Infrastructure.Dtos;
using Mapster;

namespace AzureDevOps.Infrastructure.Clients;

public class AzureDevOpsClient(IHttpClientFactory httpClientFactory) : IAzureDevOpsClient
{
    private const string ApiVersion = "7.1";
    private const string ApiVersionPreview = "7.1-preview.4";

    private static readonly JsonSerializerOptions JsonOptions = new()
    {
        PropertyNameCaseInsensitive = true
    };

    private HttpClient CreateClient() => httpClientFactory.CreateClient("AzureDevOpsApi");

    private static string WithApiVersion(string url, string? version = null)
    {
        var separator = url.Contains('?') ? '&' : '?';
        return $"{url}{separator}api-version={version ?? ApiVersion}";
    }

    // Projects
    public async Task<List<Project>> GetProjectsAsync(CancellationToken cancellationToken = default)
    {
        var http = CreateClient();
        var response = await http.GetAsync(WithApiVersion("_apis/projects"), cancellationToken);

        if (!response.IsSuccessStatusCode)
        {
            return [];
        }

        var result = await response.Content.ReadFromJsonAsync<AzureDevOpsListResponse<ProjectDto>>(JsonOptions, cancellationToken);
        return result?.Value.Select(p => p.Adapt<Project>()).ToList() ?? [];
    }

    public async Task<Project?> GetProjectAsync(string projectNameOrId, CancellationToken cancellationToken = default)
    {
        var http = CreateClient();
        var response = await http.GetAsync(WithApiVersion($"_apis/projects/{Uri.EscapeDataString(projectNameOrId)}"), cancellationToken);

        if (!response.IsSuccessStatusCode)
        {
            return null;
        }

        var dto = await response.Content.ReadFromJsonAsync<ProjectDto>(JsonOptions, cancellationToken);
        return dto?.Adapt<Project>();
    }

    // Work Items
    public async Task<WorkItem?> GetWorkItemAsync(int id, CancellationToken cancellationToken = default)
    {
        var http = CreateClient();
        var response = await http.GetAsync(WithApiVersion($"_apis/wit/workitems/{id}?$expand=all"), cancellationToken);

        if (!response.IsSuccessStatusCode)
        {
            return null;
        }

        var dto = await response.Content.ReadFromJsonAsync<WorkItemDto>(JsonOptions, cancellationToken);
        return dto?.Adapt<WorkItem>();
    }

    public async Task<WorkItem?> CreateWorkItemAsync(string project, string workItemType, string title, string? description, int? parentId, string? assignedTo, string? areaPath, string? iterationPath, string? priority, string? tags, CancellationToken cancellationToken = default)
    {
        var http = CreateClient();
        var patchOperations = new List<object>
        {
            new { op = "add", path = "/fields/System.Title", value = title }
        };

        if (description is not null)
        {
            patchOperations.Add(new { op = "add", path = "/fields/System.Description", value = description });
        }

        if (assignedTo is not null)
        {
            patchOperations.Add(new { op = "add", path = "/fields/System.AssignedTo", value = assignedTo });
        }

        if (areaPath is not null)
        {
            patchOperations.Add(new { op = "add", path = "/fields/System.AreaPath", value = areaPath });
        }

        if (iterationPath is not null)
        {
            patchOperations.Add(new { op = "add", path = "/fields/System.IterationPath", value = iterationPath });
        }

        if (priority is not null)
        {
            patchOperations.Add(new { op = "add", path = "/fields/Microsoft.VSTS.Common.Priority", value = priority });
        }

        if (tags is not null)
        {
            patchOperations.Add(new { op = "add", path = "/fields/System.Tags", value = tags });
        }

        if (parentId is not null)
        {
            patchOperations.Add(new
            {
                op = "add",
                path = "/relations/-",
                value = new
                {
                    rel = "System.LinkTypes.Hierarchy-Reverse",
                    url = $"{http.BaseAddress}_apis/wit/workitems/{parentId}"
                }
            });
        }

        var url = WithApiVersion($"{Uri.EscapeDataString(project)}/_apis/wit/workitems/${Uri.EscapeDataString(workItemType)}");
        var content = new StringContent(JsonSerializer.Serialize(patchOperations), Encoding.UTF8, "application/json-patch+json");
        var response = await http.PostAsync(url, content, cancellationToken);

        if (!response.IsSuccessStatusCode)
        {
            return null;
        }

        var dto = await response.Content.ReadFromJsonAsync<WorkItemDto>(JsonOptions, cancellationToken);
        return dto?.Adapt<WorkItem>();
    }

    public async Task<WorkItem?> UpdateWorkItemAsync(int id, string? title, string? description, string? state, string? assignedTo, string? areaPath, string? iterationPath, string? priority, string? tags, CancellationToken cancellationToken = default)
    {
        var http = CreateClient();
        var patchOperations = new List<object>();

        if (title is not null)
        {
            patchOperations.Add(new { op = "replace", path = "/fields/System.Title", value = title });
        }

        if (description is not null)
        {
            patchOperations.Add(new { op = "replace", path = "/fields/System.Description", value = description });
        }

        if (state is not null)
        {
            patchOperations.Add(new { op = "replace", path = "/fields/System.State", value = state });
        }

        if (assignedTo is not null)
        {
            patchOperations.Add(new { op = "replace", path = "/fields/System.AssignedTo", value = assignedTo });
        }

        if (areaPath is not null)
        {
            patchOperations.Add(new { op = "replace", path = "/fields/System.AreaPath", value = areaPath });
        }

        if (iterationPath is not null)
        {
            patchOperations.Add(new { op = "replace", path = "/fields/System.IterationPath", value = iterationPath });
        }

        if (priority is not null)
        {
            patchOperations.Add(new { op = "replace", path = "/fields/Microsoft.VSTS.Common.Priority", value = priority });
        }

        if (tags is not null)
        {
            patchOperations.Add(new { op = "replace", path = "/fields/System.Tags", value = tags });
        }

        if (patchOperations.Count == 0)
        {
            return await GetWorkItemAsync(id, cancellationToken);
        }

        var url = WithApiVersion($"_apis/wit/workitems/{id}");
        var content = new StringContent(JsonSerializer.Serialize(patchOperations), Encoding.UTF8, "application/json-patch+json");
        var request = new HttpRequestMessage(HttpMethod.Patch, url) { Content = content };
        var response = await http.SendAsync(request, cancellationToken);

        if (!response.IsSuccessStatusCode)
        {
            return null;
        }

        var dto = await response.Content.ReadFromJsonAsync<WorkItemDto>(JsonOptions, cancellationToken);
        return dto?.Adapt<WorkItem>();
    }

    public async Task<bool> DeleteWorkItemAsync(int id, CancellationToken cancellationToken = default)
    {
        var http = CreateClient();
        var response = await http.DeleteAsync(WithApiVersion($"_apis/wit/workitems/{id}"), cancellationToken);
        return response.IsSuccessStatusCode;
    }

    public async Task<List<WorkItem>> QueryWorkItemsAsync(string wiql, CancellationToken cancellationToken = default)
    {
        var http = CreateClient();
        var body = new { query = wiql };
        var response = await http.PostAsJsonAsync(WithApiVersion("_apis/wit/wiql"), body, cancellationToken);

        if (!response.IsSuccessStatusCode)
        {
            return [];
        }

        var result = await response.Content.ReadFromJsonAsync<WiqlResultDto>(JsonOptions, cancellationToken);

        if (result?.WorkItems is null || result.WorkItems.Count == 0)
        {
            return [];
        }

        var ids = result.WorkItems.Select(w => w.Id).Take(200).ToList();
        var idsParam = string.Join(",", ids);
        var workItemsResponse = await http.GetAsync(
            WithApiVersion($"_apis/wit/workitems?ids={idsParam}&$expand=all"),
            cancellationToken);

        if (!workItemsResponse.IsSuccessStatusCode)
        {
            return [];
        }

        var workItemsResult = await workItemsResponse.Content.ReadFromJsonAsync<AzureDevOpsListResponse<WorkItemDto>>(JsonOptions, cancellationToken);
        return workItemsResult?.Value.Select(w => w.Adapt<WorkItem>()).ToList() ?? [];
    }

    // Teams
    public async Task<List<Team>> GetTeamsAsync(string project, CancellationToken cancellationToken = default)
    {
        var http = CreateClient();
        var response = await http.GetAsync(WithApiVersion($"_apis/projects/{Uri.EscapeDataString(project)}/teams"), cancellationToken);

        if (!response.IsSuccessStatusCode)
        {
            return [];
        }

        var result = await response.Content.ReadFromJsonAsync<AzureDevOpsListResponse<TeamDto>>(JsonOptions, cancellationToken);
        return result?.Value.Select(t => t.Adapt<Team>()).ToList() ?? [];
    }

    // Boards
    public async Task<List<Board>> GetBoardsAsync(string project, string team, CancellationToken cancellationToken = default)
    {
        var http = CreateClient();
        var response = await http.GetAsync(
            WithApiVersion($"{Uri.EscapeDataString(project)}/{Uri.EscapeDataString(team)}/_apis/work/boards"),
            cancellationToken);

        if (!response.IsSuccessStatusCode)
        {
            return [];
        }

        var result = await response.Content.ReadFromJsonAsync<AzureDevOpsListResponse<BoardDto>>(JsonOptions, cancellationToken);
        return result?.Value.Select(b => b.Adapt<Board>()).ToList() ?? [];
    }

    // Sprints (Iterations)
    public async Task<List<Sprint>> GetSprintsAsync(string project, string team, CancellationToken cancellationToken = default)
    {
        var http = CreateClient();
        var response = await http.GetAsync(
            WithApiVersion($"{Uri.EscapeDataString(project)}/{Uri.EscapeDataString(team)}/_apis/work/teamsettings/iterations"),
            cancellationToken);

        if (!response.IsSuccessStatusCode)
        {
            return [];
        }

        var result = await response.Content.ReadFromJsonAsync<AzureDevOpsListResponse<SprintDto>>(JsonOptions, cancellationToken);
        return result?.Value.Select(s => s.Adapt<Sprint>()).ToList() ?? [];
    }

    // Comments
    public async Task<List<Comment>> GetCommentsAsync(string project, int workItemId, CancellationToken cancellationToken = default)
    {
        var http = CreateClient();
        var response = await http.GetAsync(
            WithApiVersion($"{Uri.EscapeDataString(project)}/_apis/wit/workitems/{workItemId}/comments", ApiVersionPreview),
            cancellationToken);

        if (!response.IsSuccessStatusCode)
        {
            return [];
        }

        var result = await response.Content.ReadFromJsonAsync<CommentListDto>(JsonOptions, cancellationToken);
        return result?.Comments.Select(c => c.Adapt<Comment>()).ToList() ?? [];
    }

    public async Task<Comment?> AddCommentAsync(string project, int workItemId, string text, CancellationToken cancellationToken = default)
    {
        var http = CreateClient();
        var body = new { text };
        var response = await http.PostAsJsonAsync(
            WithApiVersion($"{Uri.EscapeDataString(project)}/_apis/wit/workitems/{workItemId}/comments", ApiVersionPreview),
            body,
            cancellationToken);

        if (!response.IsSuccessStatusCode)
        {
            return null;
        }

        var dto = await response.Content.ReadFromJsonAsync<CommentDto>(JsonOptions, cancellationToken);
        return dto?.Adapt<Comment>();
    }
}
