using FluentResults;
using Miro.Application.Interfaces;
using Miro.Application.ResultErrors;
using Miro.Domain.Entities;

namespace Miro.Application.Services;

public interface IMiroService
{
    Task<Result<List<Board>>> GetBoardsAsync(CancellationToken cancellationToken = default);
    Task<Result<Board>> GetBoardByIdAsync(string boardId, CancellationToken cancellationToken = default);
    Task<Result<List<StickyNote>>> GetStickyNotesByBoardIdAsync(string boardId, CancellationToken cancellationToken = default);
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
}
