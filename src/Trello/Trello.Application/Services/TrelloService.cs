using FluentResults;
using Trello.Application.Interfaces;
using Trello.Application.ResultErrors;
using Trello.Domain.Entities;

namespace Trello.Application.Services;

public interface ITrelloService
{
    Task<Result<List<Board>>> GetBoardsAsync(CancellationToken cancellationToken = default);
    Task<Result<Board>> GetBoardByIdAsync(string boardId, CancellationToken cancellationToken = default);
    Task<Result<List<Card>>> GetCardsByBoardIdAsync(string boardId, CancellationToken cancellationToken = default);
    Task<Result<Card>> GetCardByIdAsync(string cardId, CancellationToken cancellationToken = default);
    Task<Result<List<Comment>>> GetCommentsByCardIdAsync(string cardId, CancellationToken cancellationToken = default);
    Task<Result> AddCommentAsync(string cardId, string text, CancellationToken cancellationToken = default);
    Task<Result<Card>> MoveCardToListAsync(string cardId, string listId, CancellationToken cancellationToken = default);
}

public class TrelloService(ITrelloClient trelloClient) : ITrelloService
{
    private const int TrelloMaxCommentLength = 16384;

    public async Task<Result<List<Board>>> GetBoardsAsync(CancellationToken cancellationToken = default)
    {
        var boards = await trelloClient.GetBoardsAsync(cancellationToken);
        return Result.Ok(boards);
    }

    public async Task<Result<Board>> GetBoardByIdAsync(string boardId, CancellationToken cancellationToken = default)
    {
        var board = await trelloClient.GetBoardByIdAsync(boardId, cancellationToken);

        if (board is null)
        {
            return Result.Fail<Board>(new EntityDoesNotExistError());
        }

        return Result.Ok(board);
    }

    public async Task<Result<List<Card>>> GetCardsByBoardIdAsync(string boardId, CancellationToken cancellationToken = default)
    {
        var cards = await trelloClient.GetCardsByBoardIdAsync(boardId, cancellationToken);
        return Result.Ok(cards);
    }

    public async Task<Result<Card>> GetCardByIdAsync(string cardId, CancellationToken cancellationToken = default)
    {
        var card = await trelloClient.GetCardByIdAsync(cardId, cancellationToken);

        if (card is null)
        {
            return Result.Fail<Card>(new EntityDoesNotExistError());
        }

        return Result.Ok(card);
    }

    public async Task<Result<List<Comment>>> GetCommentsByCardIdAsync(string cardId, CancellationToken cancellationToken = default)
    {
        var comments = await trelloClient.GetCommentsByCardIdAsync(cardId, cancellationToken);
        return Result.Ok(comments);
    }

    public async Task<Result> AddCommentAsync(string cardId, string text, CancellationToken cancellationToken = default)
    {
        var chunks = SplitComment(text);

        foreach (var chunk in chunks)
        {
            await trelloClient.AddCommentAsync(cardId, chunk, cancellationToken);
        }

        return Result.Ok();
    }

    public async Task<Result<Card>> MoveCardToListAsync(string cardId, string listId, CancellationToken cancellationToken = default)
    {
        var card = await trelloClient.MoveCardToListAsync(cardId, listId, cancellationToken);

        if (card is null)
        {
            return Result.Fail<Card>(new EntityDoesNotExistError());
        }

        return Result.Ok(card);
    }

    private static List<string> SplitComment(string comment)
    {
        if (comment.Length <= TrelloMaxCommentLength)
        {
            return [comment];
        }

        var chunks = new List<string>();
        var remaining = comment;
        var part = 1;

        while (remaining.Length > 0)
        {
            var isLast = remaining.Length <= TrelloMaxCommentLength;

            if (!isLast)
            {
                var footer = $"\n\n---\n*(continued in next comment — part {part})*";
                var maxLength = TrelloMaxCommentLength - footer.Length;

                var splitIndex = remaining.LastIndexOf('\n', maxLength);
                if (splitIndex < maxLength / 2)
                {
                    splitIndex = maxLength;
                }

                chunks.Add(remaining[..splitIndex] + footer);
                remaining = remaining[splitIndex..].TrimStart('\n');
            }
            else
            {
                chunks.Add(remaining);
                remaining = string.Empty;
            }

            part++;
        }

        return chunks;
    }
}
