using System.ComponentModel;
using System.Net.Http.Json;
using ModelContextProtocol.Server;

namespace McpServer.Tools;

[McpServerToolType]
public static class MiroTools
{
    [McpServerTool(Name = "list_miro_boards")]
    [Description("List all Miro boards for the authenticated user.")]
    public static async Task<string> ListBoards(IHttpClientFactory httpFactory)
    {
        var http = httpFactory.CreateClient("MiroApi");
        var response = await http.GetAsync("/api/v1/boards");
        return await response.Content.ReadAsStringAsync();
    }

    [McpServerTool(Name = "get_miro_board")]
    [Description("Get a Miro board by ID.")]
    public static async Task<string> GetBoard(
        IHttpClientFactory httpFactory,
        [Description("The Miro board ID")] string boardId)
    {
        var http = httpFactory.CreateClient("MiroApi");
        var response = await http.GetAsync($"/api/v1/boards/{boardId}");
        return await response.Content.ReadAsStringAsync();
    }

    [McpServerTool(Name = "list_miro_sticky_notes")]
    [Description("Get all sticky notes from a Miro board.")]
    public static async Task<string> ListStickyNotes(
        IHttpClientFactory httpFactory,
        [Description("The Miro board ID")] string boardId)
    {
        var http = httpFactory.CreateClient("MiroApi");
        var response = await http.GetAsync($"/api/v1/boards/{boardId}/sticky-notes");
        return await response.Content.ReadAsStringAsync();
    }
}
