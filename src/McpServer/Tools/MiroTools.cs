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
        return await response.ReadContentOrError();
    }

    [McpServerTool(Name = "get_miro_board")]
    [Description("Get a Miro board by ID.")]
    public static async Task<string> GetBoard(
        IHttpClientFactory httpFactory,
        [Description("The Miro board ID")] string boardId)
    {
        var http = httpFactory.CreateClient("MiroApi");
        var response = await http.GetAsync($"/api/v1/boards/{boardId}");
        return await response.ReadContentOrError();
    }

    [McpServerTool(Name = "list_miro_sticky_notes")]
    [Description("Get all sticky notes from a Miro board.")]
    public static async Task<string> ListStickyNotes(
        IHttpClientFactory httpFactory,
        [Description("The Miro board ID")] string boardId)
    {
        var http = httpFactory.CreateClient("MiroApi");
        var response = await http.GetAsync($"/api/v1/boards/{boardId}/sticky-notes");
        return await response.ReadContentOrError();
    }

    [McpServerTool(Name = "create_miro_sticky_note")]
    [Description("Create a sticky note on a Miro board. Supports content (plain text or HTML), color, shape, and position.")]
    public static async Task<string> CreateStickyNote(
        IHttpClientFactory httpFactory,
        [Description("The Miro board ID")] string boardId,
        [Description("Text content of the sticky note (supports basic HTML like <p>, <b>, <i>)")] string? content = null,
        [Description("Fill color name. Valid values: light_yellow, yellow, orange, light_green, green, dark_green, cyan, light_pink, pink, light_blue, blue, dark_blue, purple, violet, gray")] string? fillColor = null,
        [Description("Shape: 'square' or 'rectangle' (default: square)")] string? shape = null,
        [Description("X position on the board")] double? positionX = null,
        [Description("Y position on the board")] double? positionY = null)
    {
        var http = httpFactory.CreateClient("MiroApi");
        var payload = new { boardId, content, shape, fillColor, positionX, positionY };
        var response = await http.PostAsJsonAsync($"/api/v1/boards/{boardId}/sticky-notes", payload);
        return await response.ReadContentOrError();
    }

    [McpServerTool(Name = "update_miro_sticky_note")]
    [Description("Update an existing sticky note on a Miro board (content, color, position).")]
    public static async Task<string> UpdateStickyNote(
        IHttpClientFactory httpFactory,
        [Description("The Miro board ID")] string boardId,
        [Description("The sticky note item ID")] string itemId,
        [Description("New text content")] string? content = null,
        [Description("New fill color hex code")] string? fillColor = null,
        [Description("New X position")] double? positionX = null,
        [Description("New Y position")] double? positionY = null)
    {
        var http = httpFactory.CreateClient("MiroApi");
        var payload = new { content, fillColor, positionX, positionY };
        var request = new HttpRequestMessage(HttpMethod.Patch, $"/api/v1/boards/{boardId}/sticky-notes/{itemId}")
        {
            Content = JsonContent.Create(payload)
        };
        var response = await http.SendAsync(request);
        return await response.ReadContentOrError();
    }

    [McpServerTool(Name = "delete_miro_sticky_note")]
    [Description("Delete a sticky note from a Miro board.")]
    public static async Task<string> DeleteStickyNote(
        IHttpClientFactory httpFactory,
        [Description("The Miro board ID")] string boardId,
        [Description("The sticky note item ID")] string itemId)
    {
        var http = httpFactory.CreateClient("MiroApi");
        var response = await http.DeleteAsync($"/api/v1/boards/{boardId}/sticky-notes/{itemId}");
        return await response.ReadContentOrError();
    }
}
