using System.Net.Http.Json;
using System.Text.Json;
using System.Text.RegularExpressions;
using Polarion.Application.Interfaces;
using Polarion.Domain.Entities;
using Polarion.Infrastructure.Dtos;

namespace Polarion.Infrastructure.Clients;

public partial class PolarionClient(IHttpClientFactory httpClientFactory) : IPolarionClient
{
    private static readonly JsonSerializerOptions JsonOptions = new()
    {
        PropertyNameCaseInsensitive = true
    };

    private const string WorkItemFields = "title,description,type,status,priority,severity,created,updated";

    // Projects

    public async Task<List<Project>> GetProjectsAsync(CancellationToken cancellationToken = default)
    {
        var http = httpClientFactory.CreateClient("PolarionApi");
        var response = await http.GetAsync("/polarion/rest/v1/projects", cancellationToken);
        response.EnsureSuccessStatusCode();

        var result = await response.Content.ReadFromJsonAsync<PolarionJsonApiResponse<PolarionProjectDto>>(JsonOptions, cancellationToken);
        return result?.Data?.Select(MapProject).ToList() ?? [];
    }

    public async Task<Project?> GetProjectAsync(string projectId, CancellationToken cancellationToken = default)
    {
        var http = httpClientFactory.CreateClient("PolarionApi");
        var response = await http.GetAsync($"/polarion/rest/v1/projects/{projectId}", cancellationToken);

        if (!response.IsSuccessStatusCode)
        {
            return null;
        }

        var result = await response.Content.ReadFromJsonAsync<PolarionSingleResponse<PolarionProjectDto>>(JsonOptions, cancellationToken);
        return result?.Data is not null ? MapProject(result.Data) : null;
    }

    // Requirements (Work Items)

    public async Task<List<Requirement>> GetRequirementsAsync(string projectId, string? query = null, int maxResults = 50, CancellationToken cancellationToken = default)
    {
        var http = httpClientFactory.CreateClient("PolarionApi");

        var url = $"/polarion/rest/v1/projects/{projectId}/workitems?fields[workitems]={WorkItemFields}&page[size]={maxResults}";

        if (!string.IsNullOrWhiteSpace(query))
        {
            url += $"&query={Uri.EscapeDataString(query)}";
        }

        var response = await http.GetAsync(url, cancellationToken);
        response.EnsureSuccessStatusCode();

        var result = await response.Content.ReadFromJsonAsync<PolarionJsonApiResponse<PolarionWorkItemDto>>(JsonOptions, cancellationToken);
        return result?.Data?.Select(dto => MapRequirement(dto, projectId)).ToList() ?? [];
    }

    public async Task<Requirement?> GetRequirementAsync(string projectId, string workItemId, CancellationToken cancellationToken = default)
    {
        var http = httpClientFactory.CreateClient("PolarionApi");
        var response = await http.GetAsync(
            $"/polarion/rest/v1/projects/{projectId}/workitems/{workItemId}?fields[workitems]={WorkItemFields}",
            cancellationToken);

        if (!response.IsSuccessStatusCode)
        {
            return null;
        }

        var result = await response.Content.ReadFromJsonAsync<PolarionSingleResponse<PolarionWorkItemDto>>(JsonOptions, cancellationToken);
        return result?.Data is not null ? MapRequirement(result.Data, projectId) : null;
    }

    // Linked Work Items

    public async Task<List<LinkedWorkItem>> GetLinkedWorkItemsAsync(string projectId, string workItemId, CancellationToken cancellationToken = default)
    {
        var http = httpClientFactory.CreateClient("PolarionApi");
        var response = await http.GetAsync(
            $"/polarion/rest/v1/projects/{projectId}/workitems/{workItemId}/linkedworkitems",
            cancellationToken);

        if (!response.IsSuccessStatusCode)
        {
            return [];
        }

        var result = await response.Content.ReadFromJsonAsync<PolarionJsonApiResponse<PolarionLinkedWorkItemDto>>(JsonOptions, cancellationToken);
        return result?.Data?.Select(MapLinkedWorkItem).ToList() ?? [];
    }

    // Mapping helpers

    private static Project MapProject(PolarionProjectDto dto) => new()
    {
        Id = dto.Id ?? string.Empty,
        Name = dto.Attributes?.Name ?? string.Empty,
        Description = StripHtml(dto.Attributes?.Description?.Value),
        TrackerPrefix = dto.Attributes?.TrackerPrefix
    };

    private static Requirement MapRequirement(PolarionWorkItemDto dto, string projectId) => new()
    {
        Id = ExtractWorkItemId(dto.Id),
        Title = dto.Attributes?.Title ?? string.Empty,
        Description = StripHtml(dto.Attributes?.Description?.Value),
        Type = dto.Attributes?.Type,
        Status = dto.Attributes?.Status,
        Priority = dto.Attributes?.Priority,
        Severity = dto.Attributes?.Severity,
        AuthorId = dto.Relationships?.Author?.Data?.Id,
        ProjectId = projectId,
        Created = dto.Attributes?.Created is not null ? DateTime.Parse(dto.Attributes.Created) : null,
        Updated = dto.Attributes?.Updated is not null ? DateTime.Parse(dto.Attributes.Updated) : null
    };

    private static LinkedWorkItem MapLinkedWorkItem(PolarionLinkedWorkItemDto dto) => new()
    {
        WorkItemId = ExtractWorkItemId(dto.Id),
        Role = dto.Attributes?.Role,
        LinkType = dto.Type
    };

    private static string ExtractWorkItemId(string? compositeId)
    {
        if (string.IsNullOrEmpty(compositeId))
        {
            return string.Empty;
        }

        // Polarion work item IDs are in format "projectId/workItemId"
        var slashIndex = compositeId.LastIndexOf('/');
        return slashIndex >= 0 ? compositeId[(slashIndex + 1)..] : compositeId;
    }

    private static string? StripHtml(string? html)
    {
        if (string.IsNullOrWhiteSpace(html))
        {
            return null;
        }

        var text = HtmlTagRegex().Replace(html, string.Empty);
        return System.Net.WebUtility.HtmlDecode(text).Trim();
    }

    [GeneratedRegex("<[^>]+>")]
    private static partial Regex HtmlTagRegex();
}
