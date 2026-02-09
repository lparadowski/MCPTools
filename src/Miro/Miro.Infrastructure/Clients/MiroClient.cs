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
}
