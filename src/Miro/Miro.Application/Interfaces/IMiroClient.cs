using Miro.Domain.Entities;

namespace Miro.Application.Interfaces;

public interface IMiroClient
{
    Task<List<Board>> GetBoardsAsync(CancellationToken cancellationToken = default);
    Task<Board?> GetBoardByIdAsync(string boardId, CancellationToken cancellationToken = default);
    Task<List<StickyNote>> GetStickyNotesByBoardIdAsync(string boardId, CancellationToken cancellationToken = default);
    Task<StickyNote?> CreateStickyNoteAsync(string boardId, string? content, string? shape, string? fillColor, double? positionX, double? positionY, CancellationToken cancellationToken = default);
    Task<StickyNote?> UpdateStickyNoteAsync(string boardId, string itemId, string? content, string? fillColor, double? positionX, double? positionY, CancellationToken cancellationToken = default);
    Task<bool> DeleteStickyNoteAsync(string boardId, string itemId, CancellationToken cancellationToken = default);
}
