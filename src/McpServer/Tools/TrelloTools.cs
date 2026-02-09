using System.ComponentModel;
using System.Net.Http.Json;
using ModelContextProtocol.Server;

namespace McpServer.Tools;

[McpServerToolType]
public static class TrelloTools
{
    [McpServerTool(Name = "list_trello_boards")]
    [Description("List all Trello boards for the authenticated user.")]
    public static async Task<string> ListBoards(IHttpClientFactory httpFactory)
    {
        var http = httpFactory.CreateClient("TrelloApi");
        var response = await http.GetAsync("/api/v1/boards");
        return await response.Content.ReadAsStringAsync();
    }

    [McpServerTool(Name = "get_trello_board")]
    [Description("Get a Trello board by ID, including its lists/columns.")]
    public static async Task<string> GetBoard(
        IHttpClientFactory httpFactory,
        [Description("The Trello board ID")] string boardId)
    {
        var http = httpFactory.CreateClient("TrelloApi");
        var response = await http.GetAsync($"/api/v1/boards/{boardId}");
        return await response.Content.ReadAsStringAsync();
    }

    [McpServerTool(Name = "list_trello_cards")]
    [Description("List all cards on a Trello board, including which list/column each card is in.")]
    public static async Task<string> ListCards(
        IHttpClientFactory httpFactory,
        [Description("The Trello board ID")] string boardId)
    {
        var http = httpFactory.CreateClient("TrelloApi");
        var response = await http.GetAsync($"/api/v1/boards/{boardId}/cards");
        return await response.Content.ReadAsStringAsync();
    }

    [McpServerTool(Name = "get_trello_card")]
    [Description("Get a Trello card by ID, including its labels and comments.")]
    public static async Task<string> GetCard(
        IHttpClientFactory httpFactory,
        [Description("The Trello card ID")] string cardId)
    {
        var http = httpFactory.CreateClient("TrelloApi");
        var response = await http.GetAsync($"/api/v1/cards/{cardId}");
        return await response.Content.ReadAsStringAsync();
    }

    [McpServerTool(Name = "get_trello_card_comments")]
    [Description("Get all comments on a Trello card.")]
    public static async Task<string> GetCardComments(
        IHttpClientFactory httpFactory,
        [Description("The Trello card ID")] string cardId)
    {
        var http = httpFactory.CreateClient("TrelloApi");
        var response = await http.GetAsync($"/api/v1/cards/{cardId}/comments");
        return await response.Content.ReadAsStringAsync();
    }

    [McpServerTool(Name = "post_trello_comment")]
    [Description("Post a comment to a Trello card.")]
    public static async Task<string> PostComment(
        IHttpClientFactory httpFactory,
        [Description("The Trello card ID")] string cardId,
        [Description("The comment text to post")] string text)
    {
        var http = httpFactory.CreateClient("TrelloApi");
        var response = await http.PostAsJsonAsync($"/api/v1/cards/{cardId}/comments", new { text });
        return await response.Content.ReadAsStringAsync();
    }

    [McpServerTool(Name = "create_trello_card")]
    [Description("Create a new Trello card in a specified list. Use get_trello_board first to find the target list ID.")]
    public static async Task<string> CreateCard(
        IHttpClientFactory httpFactory,
        [Description("The list ID to create the card in")] string listId,
        [Description("The card title/name")] string name,
        [Description("Optional card description")] string? description = null)
    {
        var http = httpFactory.CreateClient("TrelloApi");
        var response = await http.PostAsJsonAsync("/api/v1/cards", new { listId, name, description });
        return await response.Content.ReadAsStringAsync();
    }

    [McpServerTool(Name = "move_trello_card")]
    [Description("Move a Trello card to a different list/column. Use get_trello_board first to find the target list ID.")]
    public static async Task<string> MoveCard(
        IHttpClientFactory httpFactory,
        [Description("The Trello card ID")] string cardId,
        [Description("The target list ID to move the card to")] string listId)
    {
        var http = httpFactory.CreateClient("TrelloApi");
        var response = await http.PutAsJsonAsync($"/api/v1/cards/{cardId}/list", new { listId });
        return await response.Content.ReadAsStringAsync();
    }
}
