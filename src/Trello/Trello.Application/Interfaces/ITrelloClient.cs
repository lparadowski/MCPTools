using Trello.Domain.Entities;

namespace Trello.Application.Interfaces;

public interface ITrelloClient
{
    Task<List<Board>> GetBoardsAsync(CancellationToken cancellationToken = default);
    Task<Board?> CreateBoardAsync(string name, string? description, CancellationToken cancellationToken = default);
    Task<Board?> GetBoardByIdAsync(string boardId, CancellationToken cancellationToken = default);
    Task<Board?> ArchiveBoardAsync(string boardId, CancellationToken cancellationToken = default);
    Task<bool> DeleteBoardAsync(string boardId, CancellationToken cancellationToken = default);
    Task<List<Card>> GetCardsByBoardIdAsync(string boardId, CancellationToken cancellationToken = default);
    Task<Card?> GetCardByIdAsync(string cardId, CancellationToken cancellationToken = default);
    Task<List<Comment>> GetCommentsByCardIdAsync(string cardId, CancellationToken cancellationToken = default);
    Task AddCommentAsync(string cardId, string text, CancellationToken cancellationToken = default);
    Task<Card?> MoveCardToListAsync(string cardId, string listId, CancellationToken cancellationToken = default);
    Task<Card?> CreateCardAsync(string listId, string name, string? description, CancellationToken cancellationToken = default);
    Task<Card?> UpdateCardAsync(string cardId, string? name, string? description, CancellationToken cancellationToken = default);
    Task<Card?> ArchiveCardAsync(string cardId, CancellationToken cancellationToken = default);
    Task<Card?> AddLabelToCardAsync(string cardId, string labelId, CancellationToken cancellationToken = default);
    Task<Card?> RemoveLabelFromCardAsync(string cardId, string labelId, CancellationToken cancellationToken = default);
    Task<bool> DeleteCardAsync(string cardId, CancellationToken cancellationToken = default);
    Task<List<Label>> GetBoardLabelsAsync(string boardId, CancellationToken cancellationToken = default);
    Task<Label?> CreateBoardLabelAsync(string boardId, string name, string color, CancellationToken cancellationToken = default);
}
