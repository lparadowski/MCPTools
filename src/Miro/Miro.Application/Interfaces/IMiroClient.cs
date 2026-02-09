using Miro.Domain.Entities;

namespace Miro.Application.Interfaces;

public interface IMiroClient
{
    Task<List<Board>> GetBoardsAsync(CancellationToken cancellationToken = default);
    Task<Board?> GetBoardByIdAsync(string boardId, CancellationToken cancellationToken = default);
    Task<List<StickyNote>> GetStickyNotesByBoardIdAsync(string boardId, CancellationToken cancellationToken = default);
}
