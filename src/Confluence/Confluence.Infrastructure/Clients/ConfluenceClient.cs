using System.Net.Http.Json;
using System.Text.Json;
using Confluence.Application.Interfaces;
using Confluence.Domain.Entities;
using Confluence.Infrastructure.Dtos;
using Mapster;

namespace Confluence.Infrastructure.Clients;

public class ConfluenceClient(IHttpClientFactory httpClientFactory) : IConfluenceClient
{
    private static readonly JsonSerializerOptions JsonOptions = new()
    {
        PropertyNameCaseInsensitive = true
    };

    public async Task<List<Space>> GetSpacesAsync(CancellationToken cancellationToken = default)
    {
        var http = httpClientFactory.CreateClient("ConfluenceApi");
        var allSpaces = new List<Space>();
        string? cursor = null;

        do
        {
            var url = "/wiki/api/v2/spaces?limit=25";
            if (cursor is not null)
            {
                url += $"&cursor={cursor}";
            }

            var response = await http.GetAsync(url, cancellationToken);
            response.EnsureSuccessStatusCode();

            var page = await response.Content.ReadFromJsonAsync<ConfluenceSpacesPageDto>(JsonOptions, cancellationToken);

            if (page?.Results is not null)
            {
                foreach (var space in page.Results)
                {
                    allSpaces.Add(space.Adapt<Space>());
                }
            }

            cursor = ExtractCursor(page?.Links?.Next);
        } while (cursor is not null);

        return allSpaces;
    }

    public async Task<Space?> GetSpaceByIdAsync(string spaceId, CancellationToken cancellationToken = default)
    {
        var http = httpClientFactory.CreateClient("ConfluenceApi");
        var response = await http.GetAsync($"/wiki/api/v2/spaces/{spaceId}", cancellationToken);

        if (!response.IsSuccessStatusCode)
        {
            return null;
        }

        var dto = await response.Content.ReadFromJsonAsync<ConfluenceSpaceDto>(JsonOptions, cancellationToken);

        if (dto is null)
        {
            return null;
        }

        return dto.Adapt<Space>();
    }

    public async Task<Space?> CreateSpaceAsync(string name, string key, string? description, CancellationToken cancellationToken = default)
    {
        var http = httpClientFactory.CreateClient("ConfluenceApi");

        var payload = new Dictionary<string, object?> { ["name"] = name, ["key"] = key };

        if (description is not null)
        {
            payload["description"] = new
            {
                value = description,
                representation = "plain"
            };
        }

        var response = await http.PostAsJsonAsync("/wiki/api/v2/spaces", payload, cancellationToken);

        if (!response.IsSuccessStatusCode)
        {
            return null;
        }

        var dto = await response.Content.ReadFromJsonAsync<ConfluenceSpaceDto>(JsonOptions, cancellationToken);

        if (dto is null)
        {
            return null;
        }

        return dto.Adapt<Space>();
    }

    public async Task<bool> DeleteSpaceAsync(string spaceId, CancellationToken cancellationToken = default)
    {
        var http = httpClientFactory.CreateClient("ConfluenceApi");
        var response = await http.DeleteAsync($"/wiki/api/v2/spaces/{spaceId}", cancellationToken);
        return response.IsSuccessStatusCode;
    }

    public async Task<List<Page>> GetPagesBySpaceIdAsync(string spaceId, CancellationToken cancellationToken = default)
    {
        var http = httpClientFactory.CreateClient("ConfluenceApi");
        var allPages = new List<Page>();
        string? cursor = null;

        do
        {
            var url = $"/wiki/api/v2/spaces/{spaceId}/pages?limit=25";
            if (cursor is not null)
            {
                url += $"&cursor={cursor}";
            }

            var response = await http.GetAsync(url, cancellationToken);
            response.EnsureSuccessStatusCode();

            var page = await response.Content.ReadFromJsonAsync<ConfluencePagesPageDto>(JsonOptions, cancellationToken);

            if (page?.Results is not null)
            {
                foreach (var p in page.Results)
                {
                    allPages.Add(p.Adapt<Page>());
                }
            }

            cursor = ExtractCursor(page?.Links?.Next);
        } while (cursor is not null);

        return allPages;
    }

    public async Task<Page?> GetPageByIdAsync(string pageId, CancellationToken cancellationToken = default)
    {
        var http = httpClientFactory.CreateClient("ConfluenceApi");
        var response = await http.GetAsync($"/wiki/api/v2/pages/{pageId}?body-format=storage", cancellationToken);

        if (!response.IsSuccessStatusCode)
        {
            return null;
        }

        var dto = await response.Content.ReadFromJsonAsync<ConfluencePageDto>(JsonOptions, cancellationToken);

        if (dto is null)
        {
            return null;
        }

        return dto.Adapt<Page>();
    }

    public async Task<Page?> CreatePageAsync(string spaceId, string title, string? body, string? parentId, CancellationToken cancellationToken = default)
    {
        var http = httpClientFactory.CreateClient("ConfluenceApi");

        var payload = new Dictionary<string, object?>
        {
            ["spaceId"] = spaceId,
            ["title"] = title,
            ["status"] = "current"
        };

        if (parentId is not null)
        {
            payload["parentId"] = parentId;
        }

        if (body is not null)
        {
            payload["body"] = new
            {
                representation = "storage",
                value = body
            };
        }

        var response = await http.PostAsJsonAsync("/wiki/api/v2/pages", payload, cancellationToken);

        if (!response.IsSuccessStatusCode)
        {
            return null;
        }

        var dto = await response.Content.ReadFromJsonAsync<ConfluencePageDto>(JsonOptions, cancellationToken);

        if (dto is null)
        {
            return null;
        }

        return dto.Adapt<Page>();
    }

    public async Task<Page?> UpdatePageAsync(string pageId, string title, string? body, int version, CancellationToken cancellationToken = default)
    {
        var http = httpClientFactory.CreateClient("ConfluenceApi");

        var payload = new Dictionary<string, object?>
        {
            ["id"] = pageId,
            ["title"] = title,
            ["status"] = "current",
            ["version"] = new { number = version }
        };

        if (body is not null)
        {
            payload["body"] = new
            {
                representation = "storage",
                value = body
            };
        }

        var response = await http.PutAsJsonAsync($"/wiki/api/v2/pages/{pageId}", payload, cancellationToken);

        if (!response.IsSuccessStatusCode)
        {
            return null;
        }

        var dto = await response.Content.ReadFromJsonAsync<ConfluencePageDto>(JsonOptions, cancellationToken);

        if (dto is null)
        {
            return null;
        }

        return dto.Adapt<Page>();
    }

    public async Task<bool> DeletePageAsync(string pageId, CancellationToken cancellationToken = default)
    {
        var http = httpClientFactory.CreateClient("ConfluenceApi");
        var response = await http.DeleteAsync($"/wiki/api/v2/pages/{pageId}", cancellationToken);
        return response.IsSuccessStatusCode;
    }

    private static string? ExtractCursor(string? nextLink)
    {
        if (string.IsNullOrEmpty(nextLink))
        {
            return null;
        }

        var queryIndex = nextLink.IndexOf('?');
        if (queryIndex < 0)
        {
            return null;
        }

        var queryParams = System.Web.HttpUtility.ParseQueryString(nextLink[queryIndex..]);
        return queryParams["cursor"];
    }
}
