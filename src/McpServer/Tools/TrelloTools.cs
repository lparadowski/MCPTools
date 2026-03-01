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

    [McpServerTool(Name = "update_trello_card")]
    [Description("Update a Trello card's name and/or description.")]
    public static async Task<string> UpdateCard(
        IHttpClientFactory httpFactory,
        [Description("The Trello card ID")] string cardId,
        [Description("New name for the card (null to keep current)")] string? name = null,
        [Description("New description for the card (null to keep current)")] string? description = null)
    {
        var http = httpFactory.CreateClient("TrelloApi");
        var response = await http.PutAsJsonAsync($"/api/v1/cards/{cardId}", new { name, description });
        return await response.Content.ReadAsStringAsync();
    }

    [McpServerTool(Name = "archive_trello_card")]
    [Description("Archive (close) a Trello card. This removes it from the board but does not permanently delete it.")]
    public static async Task<string> ArchiveCard(
        IHttpClientFactory httpFactory,
        [Description("The Trello card ID")] string cardId)
    {
        var http = httpFactory.CreateClient("TrelloApi");
        var response = await http.PutAsync($"/api/v1/cards/{cardId}/archive", null);
        return await response.Content.ReadAsStringAsync();
    }

    [McpServerTool(Name = "delete_trello_card")]
    [Description("Permanently delete a Trello card. This cannot be undone. Use archive_trello_card if you want to keep the card recoverable.")]
    public static async Task<string> DeleteCard(
        IHttpClientFactory httpFactory,
        [Description("The Trello card ID")] string cardId)
    {
        var http = httpFactory.CreateClient("TrelloApi");
        var response = await http.DeleteAsync($"/api/v1/cards/{cardId}");
        return await response.Content.ReadAsStringAsync();
    }

    [McpServerTool(Name = "list_trello_board_labels")]
    [Description("List all labels defined on a Trello board. Returns label IDs, names, and colors.")]
    public static async Task<string> ListBoardLabels(
        IHttpClientFactory httpFactory,
        [Description("The Trello board ID")] string boardId)
    {
        var http = httpFactory.CreateClient("TrelloApi");
        var response = await http.GetAsync($"/api/v1/boards/{boardId}/labels");
        return await response.Content.ReadAsStringAsync();
    }

    [McpServerTool(Name = "create_trello_board_label")]
    [Description("Create a new label on a Trello board. Valid colors: Green, Yellow, Orange, Red, Purple, Blue, Sky, Lime, Pink, Black.")]
    public static async Task<string> CreateBoardLabel(
        IHttpClientFactory httpFactory,
        [Description("The Trello board ID")] string boardId,
        [Description("The label name")] string name,
        [Description("The label color (e.g. Green, Red, Blue)")] string color)
    {
        var http = httpFactory.CreateClient("TrelloApi");
        var response = await http.PostAsJsonAsync($"/api/v1/boards/{boardId}/labels", new { name, color });
        return await response.Content.ReadAsStringAsync();
    }

    [McpServerTool(Name = "add_label_to_trello_card")]
    [Description("Add a label to a Trello card. Use list_trello_board_labels to find available label IDs.")]
    public static async Task<string> AddLabel(
        IHttpClientFactory httpFactory,
        [Description("The Trello card ID")] string cardId,
        [Description("The label ID to add")] string labelId)
    {
        var http = httpFactory.CreateClient("TrelloApi");
        var response = await http.PostAsJsonAsync($"/api/v1/cards/{cardId}/labels", new { labelId });
        return await response.Content.ReadAsStringAsync();
    }

    [McpServerTool(Name = "remove_label_from_trello_card")]
    [Description("Remove a label from a Trello card.")]
    public static async Task<string> RemoveLabel(
        IHttpClientFactory httpFactory,
        [Description("The Trello card ID")] string cardId,
        [Description("The label ID to remove")] string labelId)
    {
        var http = httpFactory.CreateClient("TrelloApi");
        var response = await http.DeleteAsync($"/api/v1/cards/{cardId}/labels/{labelId}");
        return await response.Content.ReadAsStringAsync();
    }
}
