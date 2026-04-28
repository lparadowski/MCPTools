using System.Net.Http.Json;
using System.Text.Json;
using Mapster;
using Trello.Application.Interfaces;
using Trello.Domain.Entities;
using Trello.Infrastructure.Dtos;

namespace Trello.Infrastructure.Clients;

public class TrelloClient(IHttpClientFactory httpClientFactory) : ITrelloClient
{
    private static readonly JsonSerializerOptions JsonOptions = new()
    {
        PropertyNameCaseInsensitive = true
    };

    // Boards

    public async Task<List<Board>> GetBoardsAsync(CancellationToken cancellationToken = default)
    {
        var http = httpClientFactory.CreateClient("TrelloApi");
        var response = await http.GetAsync("/1/members/me/boards?filter=open&fields=name,desc,url", cancellationToken);
        response.EnsureSuccessStatusCode();

        var dtos = await response.Content.ReadFromJsonAsync<List<TrelloBoardDto>>(JsonOptions, cancellationToken);
        return dtos?.Select(b => b.Adapt<Board>()).ToList() ?? [];
    }

    public async Task<Board?> CreateBoardAsync(string name, string? description, CancellationToken cancellationToken = default)
    {
        var http = httpClientFactory.CreateClient("TrelloApi");
        var query = $"/1/boards?name={Uri.EscapeDataString(name)}";
        if (description is not null)
        {
            query += $"&desc={Uri.EscapeDataString(description)}";
        }

        var response = await http.PostAsync(query, null, cancellationToken);

        if (!response.IsSuccessStatusCode)
        {
            return null;
        }

        var dto = await response.Content.ReadFromJsonAsync<TrelloBoardDto>(JsonOptions, cancellationToken);
        return dto?.Adapt<Board>();
    }

    public async Task<Board?> GetBoardByIdAsync(string boardId, CancellationToken cancellationToken = default)
    {
        var http = httpClientFactory.CreateClient("TrelloApi");
        var response = await http.GetAsync($"/1/boards/{boardId}?fields=name,desc,url", cancellationToken);

        if (!response.IsSuccessStatusCode)
        {
            return null;
        }

        var dto = await response.Content.ReadFromJsonAsync<TrelloBoardDto>(JsonOptions, cancellationToken);
        if (dto is null)
        {
            return null;
        }

        var board = dto.Adapt<Board>();

        var listsResponse = await http.GetAsync($"/1/boards/{boardId}/lists?fields=name", cancellationToken);
        listsResponse.EnsureSuccessStatusCode();

        var lists = await listsResponse.Content.ReadFromJsonAsync<List<TrelloListDto>>(JsonOptions, cancellationToken);
        board.Lists = lists?.Select(l => l.Adapt<BoardList>()).ToList() ?? [];

        return board;
    }

    public async Task<Board?> ArchiveBoardAsync(string boardId, CancellationToken cancellationToken = default)
    {
        var http = httpClientFactory.CreateClient("TrelloApi");
        var response = await http.PutAsync($"/1/boards/{boardId}?closed=true", null, cancellationToken);

        if (!response.IsSuccessStatusCode)
        {
            return null;
        }

        var dto = await response.Content.ReadFromJsonAsync<TrelloBoardDto>(JsonOptions, cancellationToken);
        return dto?.Adapt<Board>();
    }

    public async Task<bool> DeleteBoardAsync(string boardId, CancellationToken cancellationToken = default)
    {
        var http = httpClientFactory.CreateClient("TrelloApi");
        var response = await http.DeleteAsync($"/1/boards/{boardId}", cancellationToken);
        return response.IsSuccessStatusCode;
    }

    // Cards

    public async Task<List<Card>> GetCardsByBoardIdAsync(string boardId, CancellationToken cancellationToken = default)
    {
        var http = httpClientFactory.CreateClient("TrelloApi");
        var response = await http.GetAsync(
            $"/1/boards/{boardId}/cards?fields=name,desc,idList,shortUrl,dateLastActivity&attachments=false",
            cancellationToken);
        response.EnsureSuccessStatusCode();

        var dtos = await response.Content.ReadFromJsonAsync<List<TrelloCardDto>>(JsonOptions, cancellationToken);

        // Fetch lists for list name mapping
        var listsResponse = await http.GetAsync($"/1/boards/{boardId}/lists?fields=name", cancellationToken);
        listsResponse.EnsureSuccessStatusCode();
        var lists = await listsResponse.Content.ReadFromJsonAsync<List<TrelloListDto>>(JsonOptions, cancellationToken);
        var listMap = lists?.ToDictionary(l => l.Id ?? string.Empty, l => l.Name ?? string.Empty) ?? [];

        return dtos?.Select(c =>
        {
            var card = c.Adapt<Card>();
            if (c.IdList is not null && listMap.TryGetValue(c.IdList, out var listName))
            {
                card.ListName = listName;
            }

            return card;
        }).ToList() ?? [];
    }

    public async Task<Card?> GetCardByIdAsync(string cardId, CancellationToken cancellationToken = default)
    {
        var http = httpClientFactory.CreateClient("TrelloApi");
        var response = await http.GetAsync(
            $"/1/cards/{cardId}?fields=name,desc,idList,shortUrl,dateLastActivity&labels=all",
            cancellationToken);

        if (!response.IsSuccessStatusCode)
        {
            return null;
        }

        var dto = await response.Content.ReadFromJsonAsync<TrelloCardDto>(JsonOptions, cancellationToken);
        if (dto is null)
        {
            return null;
        }

        var card = dto.Adapt<Card>();

        // Fetch list name
        if (dto.IdList is not null)
        {
            var listResponse = await http.GetAsync($"/1/lists/{dto.IdList}?fields=name", cancellationToken);
            if (listResponse.IsSuccessStatusCode)
            {
                var listDto = await listResponse.Content.ReadFromJsonAsync<TrelloListDto>(JsonOptions, cancellationToken);
                card.ListName = listDto?.Name;
            }
        }

        // Fetch comments
        var commentsResponse = await http.GetAsync(
            $"/1/cards/{cardId}/actions?filter=commentCard&fields=data,date",
            cancellationToken);
        commentsResponse.EnsureSuccessStatusCode();

        var comments = await commentsResponse.Content.ReadFromJsonAsync<List<TrelloActionDto>>(JsonOptions, cancellationToken);
        card.Comments = comments?.Select(c => c.Adapt<Comment>()).ToList() ?? [];

        return card;
    }

    public async Task<Card?> CreateCardAsync(string listId, string name, string? description, CancellationToken cancellationToken = default)
    {
        var http = httpClientFactory.CreateClient("TrelloApi");
        var query = $"/1/cards?idList={Uri.EscapeDataString(listId)}&name={Uri.EscapeDataString(name)}";
        if (description is not null)
        {
            query += $"&desc={Uri.EscapeDataString(description)}";
        }

        var response = await http.PostAsync(query, null, cancellationToken);

        if (!response.IsSuccessStatusCode)
        {
            return null;
        }

        var dto = await response.Content.ReadFromJsonAsync<TrelloCardDto>(JsonOptions, cancellationToken);
        return dto?.Adapt<Card>();
    }

    public async Task<Card?> UpdateCardAsync(string cardId, string? name, string? description, CancellationToken cancellationToken = default)
    {
        var http = httpClientFactory.CreateClient("TrelloApi");
        var parts = new List<string>();
        if (name is not null)
        {
            parts.Add($"name={Uri.EscapeDataString(name)}");
        }

        if (description is not null)
        {
            parts.Add($"desc={Uri.EscapeDataString(description)}");
        }

        var response = await http.PutAsync($"/1/cards/{cardId}?{string.Join("&", parts)}", null, cancellationToken);

        if (!response.IsSuccessStatusCode)
        {
            return null;
        }

        var dto = await response.Content.ReadFromJsonAsync<TrelloCardDto>(JsonOptions, cancellationToken);
        return dto?.Adapt<Card>();
    }

    public async Task<Card?> MoveCardToListAsync(string cardId, string listId, CancellationToken cancellationToken = default)
    {
        var http = httpClientFactory.CreateClient("TrelloApi");
        var response = await http.PutAsync(
            $"/1/cards/{cardId}?idList={Uri.EscapeDataString(listId)}",
            null, cancellationToken);

        if (!response.IsSuccessStatusCode)
        {
            return null;
        }

        var dto = await response.Content.ReadFromJsonAsync<TrelloCardDto>(JsonOptions, cancellationToken);
        if (dto is null)
        {
            return null;
        }

        var card = dto.Adapt<Card>();

        // Fetch new list name
        var listResponse = await http.GetAsync($"/1/lists/{listId}?fields=name", cancellationToken);
        if (listResponse.IsSuccessStatusCode)
        {
            var listDto = await listResponse.Content.ReadFromJsonAsync<TrelloListDto>(JsonOptions, cancellationToken);
            card.ListName = listDto?.Name;
        }

        return card;
    }

    public async Task<Card?> ArchiveCardAsync(string cardId, CancellationToken cancellationToken = default)
    {
        var http = httpClientFactory.CreateClient("TrelloApi");
        var response = await http.PutAsync($"/1/cards/{cardId}?closed=true", null, cancellationToken);

        if (!response.IsSuccessStatusCode)
        {
            return null;
        }

        var dto = await response.Content.ReadFromJsonAsync<TrelloCardDto>(JsonOptions, cancellationToken);
        return dto?.Adapt<Card>();
    }

    public async Task<bool> DeleteCardAsync(string cardId, CancellationToken cancellationToken = default)
    {
        var http = httpClientFactory.CreateClient("TrelloApi");
        var response = await http.DeleteAsync($"/1/cards/{cardId}", cancellationToken);
        return response.IsSuccessStatusCode;
    }

    // Comments

    public async Task<List<Comment>> GetCommentsByCardIdAsync(string cardId, CancellationToken cancellationToken = default)
    {
        var http = httpClientFactory.CreateClient("TrelloApi");
        var response = await http.GetAsync(
            $"/1/cards/{cardId}/actions?filter=commentCard&fields=data,date",
            cancellationToken);
        response.EnsureSuccessStatusCode();

        var dtos = await response.Content.ReadFromJsonAsync<List<TrelloActionDto>>(JsonOptions, cancellationToken);
        return dtos?.Select(c => c.Adapt<Comment>()).ToList() ?? [];
    }

    public async Task AddCommentAsync(string cardId, string text, CancellationToken cancellationToken = default)
    {
        var http = httpClientFactory.CreateClient("TrelloApi");
        var response = await http.PostAsync(
            $"/1/cards/{cardId}/actions/comments?text={Uri.EscapeDataString(text)}",
            null, cancellationToken);
        response.EnsureSuccessStatusCode();
    }

    // Labels

    public async Task<List<Label>> GetBoardLabelsAsync(string boardId, CancellationToken cancellationToken = default)
    {
        var http = httpClientFactory.CreateClient("TrelloApi");
        var response = await http.GetAsync($"/1/boards/{boardId}/labels?fields=name,color", cancellationToken);
        response.EnsureSuccessStatusCode();

        var dtos = await response.Content.ReadFromJsonAsync<List<TrelloLabelDto>>(JsonOptions, cancellationToken);
        return dtos?.Select(l => l.Adapt<Label>()).ToList() ?? [];
    }

    public async Task<Label?> CreateBoardLabelAsync(string boardId, string name, string color, CancellationToken cancellationToken = default)
    {
        var http = httpClientFactory.CreateClient("TrelloApi");
        var response = await http.PostAsync(
            $"/1/boards/{boardId}/labels?name={Uri.EscapeDataString(name)}&color={Uri.EscapeDataString(color.ToLowerInvariant())}",
            null, cancellationToken);

        if (!response.IsSuccessStatusCode)
        {
            return null;
        }

        var dto = await response.Content.ReadFromJsonAsync<TrelloLabelDto>(JsonOptions, cancellationToken);
        return dto?.Adapt<Label>();
    }

    public async Task<Card?> AddLabelToCardAsync(string cardId, string labelId, CancellationToken cancellationToken = default)
    {
        var http = httpClientFactory.CreateClient("TrelloApi");
        var response = await http.PostAsync(
            $"/1/cards/{cardId}/idLabels?value={Uri.EscapeDataString(labelId)}",
            null, cancellationToken);

        if (!response.IsSuccessStatusCode)
        {
            return null;
        }

        return await GetCardByIdAsync(cardId, cancellationToken);
    }

    public async Task<Card?> RemoveLabelFromCardAsync(string cardId, string labelId, CancellationToken cancellationToken = default)
    {
        var http = httpClientFactory.CreateClient("TrelloApi");
        var response = await http.DeleteAsync($"/1/cards/{cardId}/idLabels/{labelId}", cancellationToken);

        if (!response.IsSuccessStatusCode)
        {
            return null;
        }

        return await GetCardByIdAsync(cardId, cancellationToken);
    }
}
