using System.Net.Http.Json;
using Mapster;
using Miro.Application.Interfaces;
using Miro.Domain.Entities;
using Miro.Infrastructure.Dtos;

namespace Miro.Infrastructure.Clients;

public class MiroClient(IHttpClientFactory httpClientFactory) : IMiroClient
{
    public async Task<List<Board>> GetBoardsAsync(CancellationToken cancellationToken = default)
    {
        var http = httpClientFactory.CreateClient("MiroApi");
        var allBoards = new List<Board>();
        string? cursor = null;

        do
        {
            var url = "/v2/boards?limit=50";
            if (cursor is not null)
            {
                url += $"&cursor={cursor}";
            }

            var response = await http.GetAsync(url, cancellationToken);
            response.EnsureSuccessStatusCode();

            var page = await response.Content.ReadFromJsonAsync<MiroBoardsPageDto>(cancellationToken: cancellationToken);

            if (page?.Data is not null)
            {
                foreach (var board in page.Data)
                {
                    allBoards.Add(board.Adapt<Board>());
                }
            }

            cursor = page?.Cursor;
        } while (cursor is not null);

        return allBoards;
    }

    public async Task<Board?> GetBoardByIdAsync(string boardId, CancellationToken cancellationToken = default)
    {
        var http = httpClientFactory.CreateClient("MiroApi");
        var response = await http.GetAsync($"/v2/boards/{Uri.EscapeDataString(boardId)}", cancellationToken);

        if (!response.IsSuccessStatusCode)
        {
            return null;
        }

        var miroBoard = await response.Content.ReadFromJsonAsync<MiroBoardDto>(cancellationToken: cancellationToken);

        if (miroBoard is null)
        {
            return null;
        }

        return miroBoard.Adapt<Board>();
    }

    public async Task<List<StickyNote>> GetStickyNotesByBoardIdAsync(string boardId, CancellationToken cancellationToken = default)
    {
        var http = httpClientFactory.CreateClient("MiroApi");
        var allNotes = new List<StickyNote>();
        string? cursor = null;

        do
        {
            var url = $"/v2/boards/{Uri.EscapeDataString(boardId)}/items?type=sticky_note&limit=50";
            if (cursor is not null)
            {
                url += $"&cursor={cursor}";
            }

            var response = await http.GetAsync(url, cancellationToken);
            response.EnsureSuccessStatusCode();

            var page = await response.Content.ReadFromJsonAsync<MiroItemsPageDto>(cancellationToken: cancellationToken);

            if (page?.Data is not null)
            {
                foreach (var item in page.Data)
                {
                    allNotes.Add(item.Adapt<StickyNote>());
                }
            }

            cursor = page?.Cursor;
        } while (cursor is not null);

        return allNotes;
    }

    public async Task<StickyNote?> CreateStickyNoteAsync(string boardId, string? content, string? shape, string? fillColor, double? positionX, double? positionY, CancellationToken cancellationToken = default)
    {
        var http = httpClientFactory.CreateClient("MiroApi");

        var payload = new Dictionary<string, object?>();

        if (content is not null || shape is not null)
        {
            var data = new Dictionary<string, object?>();
            if (content is not null)
            {
                data["content"] = content;
            }

            if (shape is not null)
            {
                data["shape"] = shape;
            }

            payload["data"] = data;
        }

        if (fillColor is not null)
        {
            payload["style"] = new { fillColor };
        }

        if (positionX is not null || positionY is not null)
        {
            payload["position"] = new { x = positionX ?? 0, y = positionY ?? 0, origin = "center" };
        }

        var response = await http.PostAsJsonAsync($"/v2/boards/{Uri.EscapeDataString(boardId)}/sticky_notes", payload, cancellationToken);

        if (!response.IsSuccessStatusCode)
        {
            return null;
        }

        var dto = await response.Content.ReadFromJsonAsync<MiroItemDto>(cancellationToken: cancellationToken);
        return dto?.Adapt<StickyNote>();
    }

    public async Task<StickyNote?> UpdateStickyNoteAsync(string boardId, string itemId, string? content, string? fillColor, double? positionX, double? positionY, CancellationToken cancellationToken = default)
    {
        var http = httpClientFactory.CreateClient("MiroApi");

        var payload = new Dictionary<string, object?>();

        if (content is not null)
        {
            payload["data"] = new { content };
        }

        if (fillColor is not null)
        {
            payload["style"] = new { fillColor };
        }

        if (positionX is not null || positionY is not null)
        {
            payload["position"] = new { x = positionX ?? 0, y = positionY ?? 0, origin = "center" };
        }

        var request = new HttpRequestMessage(HttpMethod.Patch, $"/v2/boards/{Uri.EscapeDataString(boardId)}/sticky_notes/{itemId}")
        {
            Content = JsonContent.Create(payload)
        };

        var response = await http.SendAsync(request, cancellationToken);

        if (!response.IsSuccessStatusCode)
        {
            return null;
        }

        var dto = await response.Content.ReadFromJsonAsync<MiroItemDto>(cancellationToken: cancellationToken);
        return dto?.Adapt<StickyNote>();
    }

    public async Task<bool> DeleteStickyNoteAsync(string boardId, string itemId, CancellationToken cancellationToken = default)
    {
        var http = httpClientFactory.CreateClient("MiroApi");
        var response = await http.DeleteAsync($"/v2/boards/{Uri.EscapeDataString(boardId)}/sticky_notes/{itemId}", cancellationToken);
        return response.IsSuccessStatusCode;
    }
}
