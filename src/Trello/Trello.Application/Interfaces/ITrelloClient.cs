using Trello.Domain.Entities;

namespace Trello.Application.Interfaces;

public interface ITrelloClient
{
    Task<List<Board>> GetBoardsAsync(CancellationToken cancellationToken = default);
    Task<Board?> GetBoardByIdAsync(string boardId, CancellationToken cancellationToken = default);
    Task<List<Card>> GetCardsByBoardIdAsync(string boardId, CancellationToken cancellationToken = default);
    Task<Card?> GetCardByIdAsync(string cardId, CancellationToken cancellationToken = default);
    Task<List<Comment>> GetCommentsByCardIdAsync(string cardId, CancellationToken cancellationToken = default);
    Task AddCommentAsync(string cardId, string text, CancellationToken cancellationToken = default);
    Task<Card?> MoveCardToListAsync(string cardId, string listId, CancellationToken cancellationToken = default);
    Task<Card?> CreateCardAsync(string listId, string name, string? description, CancellationToken cancellationToken = default);
}
