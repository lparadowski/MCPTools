using System.ComponentModel;
using System.Net.Http.Json;
using ModelContextProtocol.Server;

namespace McpServer.Tools;

[McpServerToolType]
public static class ConfluenceTools
{
    [McpServerTool(Name = "list_confluence_spaces")]
    [Description("List all Confluence spaces for the authenticated user.")]
    public static async Task<string> ListSpaces(IHttpClientFactory httpFactory)
    {
        var http = httpFactory.CreateClient("ConfluenceApi");
        var response = await http.GetAsync("/api/v1/spaces");
        return await response.ReadContentOrError();
    }

    [McpServerTool(Name = "get_confluence_space")]
    [Description("Get a Confluence space by ID.")]
    public static async Task<string> GetSpace(
        IHttpClientFactory httpFactory,
        [Description("The Confluence space ID")] string spaceId)
    {
        var http = httpFactory.CreateClient("ConfluenceApi");
        var response = await http.GetAsync($"/api/v1/spaces/{spaceId}");
        return await response.ReadContentOrError();
    }

    [McpServerTool(Name = "create_confluence_space")]
    [Description("Create a new Confluence space. Requires a unique space key (uppercase, no spaces).")]
    public static async Task<string> CreateSpace(
        IHttpClientFactory httpFactory,
        [Description("The space name")] string name,
        [Description("Unique space key (uppercase, no spaces, e.g. 'MYSPACE')")] string key,
        [Description("Optional space description")] string? description = null)
    {
        var http = httpFactory.CreateClient("ConfluenceApi");
        var response = await http.PostAsJsonAsync("/api/v1/spaces", new { name, key, description });
        return await response.ReadContentOrError();
    }

    [McpServerTool(Name = "delete_confluence_space")]
    [Description("Permanently delete a Confluence space. This cannot be undone.")]
    public static async Task<string> DeleteSpace(
        IHttpClientFactory httpFactory,
        [Description("The Confluence space ID")] string spaceId)
    {
        var http = httpFactory.CreateClient("ConfluenceApi");
        var response = await http.DeleteAsync($"/api/v1/spaces/{spaceId}");
        return await response.ReadContentOrError();
    }

    [McpServerTool(Name = "list_confluence_pages")]
    [Description("List all pages in a Confluence space.")]
    public static async Task<string> ListPages(
        IHttpClientFactory httpFactory,
        [Description("The Confluence space ID")] string spaceId)
    {
        var http = httpFactory.CreateClient("ConfluenceApi");
        var response = await http.GetAsync($"/api/v1/pages/by-space/{spaceId}");
        return await response.ReadContentOrError();
    }

    [McpServerTool(Name = "get_confluence_page")]
    [Description("Get a Confluence page by ID, including its body content and version number. Large pages are returned in chunks — use offset/maxLength to paginate through the body content. If hasMore is true, call again with the nextOffset value.")]
    public static async Task<string> GetPage(
        IHttpClientFactory httpFactory,
        [Description("The Confluence page ID")] string pageId,
        [Description("Character offset into the body content. Default: 0")] int offset = 0,
        [Description("Max characters to return in the body. 0 = use server-configured default")] int maxLength = 0)
    {
        var http = httpFactory.CreateClient("ConfluenceApi");
        var response = await http.GetAsync($"/api/v1/pages/{pageId}?offset={offset}&maxLength={maxLength}");
        return await response.ReadContentOrError();
    }

    [McpServerTool(Name = "create_confluence_page")]
    [Description("Create a new Confluence page. Body uses Confluence storage format (HTML-like).")]
    public static async Task<string> CreatePage(
        IHttpClientFactory httpFactory,
        [Description("The space ID to create the page in")] string spaceId,
        [Description("The page title")] string title,
        [Description("Page body in Confluence storage format (HTML-like)")] string? body = null,
        [Description("Parent page ID for nesting under another page")] string? parentId = null)
    {
        var http = httpFactory.CreateClient("ConfluenceApi");
        var response = await http.PostAsJsonAsync("/api/v1/pages", new { spaceId, title, body, parentId });
        return await response.ReadContentOrError();
    }

    [McpServerTool(Name = "update_confluence_page")]
    [Description("Update a Confluence page's title and/or body. Requires the current version number (get it from get_confluence_page). The version must be incremented by 1.")]
    public static async Task<string> UpdatePage(
        IHttpClientFactory httpFactory,
        [Description("The page ID to update")] string pageId,
        [Description("The new page title")] string title,
        [Description("The new version number (current version + 1)")] int version,
        [Description("New page body in Confluence storage format")] string? body = null)
    {
        var http = httpFactory.CreateClient("ConfluenceApi");
        var response = await http.PutAsJsonAsync($"/api/v1/pages/{pageId}", new { title, body, version });
        return await response.ReadContentOrError();
    }

    [McpServerTool(Name = "search_confluence")]
    [Description("Search Confluence using CQL (Confluence Query Language). Examples: 'text ~ \"deploy process\"', 'type = page AND space = DEV', 'label = architecture AND lastModified > now(\"-30d\")'")]
    public static async Task<string> Search(
        IHttpClientFactory httpFactory,
        [Description("CQL query string (e.g. 'text ~ \"search term\"' or 'type = page AND space = DEV')")] string cql,
        [Description("Maximum results to return (default 25)")] int maxResults = 25)
    {
        var http = httpFactory.CreateClient("ConfluenceApi");
        var response = await http.PostAsJsonAsync("/api/v1/search", new { cql, maxResults });
        return await response.ReadContentOrError();
    }

    [McpServerTool(Name = "delete_confluence_page")]
    [Description("Permanently delete a Confluence page. This cannot be undone.")]
    public static async Task<string> DeletePage(
        IHttpClientFactory httpFactory,
        [Description("The Confluence page ID")] string pageId)
    {
        var http = httpFactory.CreateClient("ConfluenceApi");
        var response = await http.DeleteAsync($"/api/v1/pages/{pageId}");
        return await response.ReadContentOrError();
    }
}
