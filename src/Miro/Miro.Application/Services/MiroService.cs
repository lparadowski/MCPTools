using FluentResults;
using Miro.Application.Interfaces;
using Shared.Application.ResultErrors;
using Miro.Domain.Entities;

namespace Miro.Application.Services;

public interface IMiroService
{
    Task<Result<List<Board>>> GetBoardsAsync(CancellationToken cancellationToken = default);
    Task<Result<Board>> GetBoardByIdAsync(string boardId, CancellationToken cancellationToken = default);
    Task<Result<List<StickyNote>>> GetStickyNotesByBoardIdAsync(string boardId, CancellationToken cancellationToken = default);
    Task<Result<StickyNote>> CreateStickyNoteAsync(string boardId, string? content, string? shape, string? fillColor, double? positionX, double? positionY, CancellationToken cancellationToken = default);
    Task<Result<StickyNote>> UpdateStickyNoteAsync(string boardId, string itemId, string? content, string? fillColor, double? positionX, double? positionY, CancellationToken cancellationToken = default);
    Task<Result<bool>> DeleteStickyNoteAsync(string boardId, string itemId, CancellationToken cancellationToken = default);
}

public class MiroService(IMiroClient miroClient) : IMiroService
{
    public async Task<Result<List<Board>>> GetBoardsAsync(CancellationToken cancellationToken = default)
    {
        var boards = await miroClient.GetBoardsAsync(cancellationToken);
        return Result.Ok(boards);
    }

    public async Task<Result<Board>> GetBoardByIdAsync(string boardId, CancellationToken cancellationToken = default)
    {
        var board = await miroClient.GetBoardByIdAsync(boardId, cancellationToken);

        if (board is null)
        {
            return Result.Fail<Board>(new EntityDoesNotExistError());
        }

        return Result.Ok(board);
    }

    public async Task<Result<List<StickyNote>>> GetStickyNotesByBoardIdAsync(string boardId, CancellationToken cancellationToken = default)
    {
        var stickyNotes = await miroClient.GetStickyNotesByBoardIdAsync(boardId, cancellationToken);
        return Result.Ok(stickyNotes);
    }

    public async Task<Result<StickyNote>> CreateStickyNoteAsync(string boardId, string? content, string? shape, string? fillColor, double? positionX, double? positionY, CancellationToken cancellationToken = default)
    {
        var stickyNote = await miroClient.CreateStickyNoteAsync(boardId, content, shape, fillColor, positionX, positionY, cancellationToken);

        if (stickyNote is null)
        {
            return Result.Fail<StickyNote>(new EntityDoesNotExistError());
        }

        return Result.Ok(stickyNote);
    }

    public async Task<Result<StickyNote>> UpdateStickyNoteAsync(string boardId, string itemId, string? content, string? fillColor, double? positionX, double? positionY, CancellationToken cancellationToken = default)
    {
        var stickyNote = await miroClient.UpdateStickyNoteAsync(boardId, itemId, content, fillColor, positionX, positionY, cancellationToken);

        if (stickyNote is null)
        {
            return Result.Fail<StickyNote>(new EntityDoesNotExistError());
        }

        return Result.Ok(stickyNote);
    }

    public async Task<Result<bool>> DeleteStickyNoteAsync(string boardId, string itemId, CancellationToken cancellationToken = default)
    {
        var success = await miroClient.DeleteStickyNoteAsync(boardId, itemId, cancellationToken);
        return Result.Ok(success);
    }
}
