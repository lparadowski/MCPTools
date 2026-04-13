using FluentResults;
using Trello.Application.Interfaces;
using Shared.Application.ResultErrors;
using Trello.Domain.Entities;

namespace Trello.Application.Services;

public interface ITrelloService
{
    Task<Result<List<Board>>> GetBoardsAsync(CancellationToken cancellationToken = default);
    Task<Result<Board>> CreateBoardAsync(string name, string? description, CancellationToken cancellationToken = default);
    Task<Result<Board>> GetBoardByIdAsync(string boardId, CancellationToken cancellationToken = default);
    Task<Result<Board>> ArchiveBoardAsync(string boardId, CancellationToken cancellationToken = default);
    Task<Result> DeleteBoardAsync(string boardId, CancellationToken cancellationToken = default);
    Task<Result<List<Card>>> GetCardsByBoardIdAsync(string boardId, CancellationToken cancellationToken = default);
    Task<Result<Card>> GetCardByIdAsync(string cardId, CancellationToken cancellationToken = default);
    Task<Result<List<Comment>>> GetCommentsByCardIdAsync(string cardId, CancellationToken cancellationToken = default);
    Task<Result> AddCommentAsync(string cardId, string text, CancellationToken cancellationToken = default);
    Task<Result<Card>> MoveCardToListAsync(string cardId, string listId, CancellationToken cancellationToken = default);
    Task<Result<Card>> CreateCardAsync(string listId, string name, string? description, CancellationToken cancellationToken = default);
    Task<Result<Card>> UpdateCardAsync(string cardId, string? name, string? description, CancellationToken cancellationToken = default);
    Task<Result<Card>> ArchiveCardAsync(string cardId, CancellationToken cancellationToken = default);
    Task<Result<Card>> AddLabelToCardAsync(string cardId, string labelId, CancellationToken cancellationToken = default);
    Task<Result<Card>> RemoveLabelFromCardAsync(string cardId, string labelId, CancellationToken cancellationToken = default);
    Task<Result> DeleteCardAsync(string cardId, CancellationToken cancellationToken = default);
    Task<Result<List<Label>>> GetBoardLabelsAsync(string boardId, CancellationToken cancellationToken = default);
    Task<Result<Label>> CreateBoardLabelAsync(string boardId, string name, string color, CancellationToken cancellationToken = default);
}

public class TrelloService(ITrelloClient trelloClient) : ITrelloService
{
    private const int TrelloMaxCommentLength = 16384;

    public async Task<Result<List<Board>>> GetBoardsAsync(CancellationToken cancellationToken = default)
    {
        var boards = await trelloClient.GetBoardsAsync(cancellationToken);
        return Result.Ok(boards);
    }

    public async Task<Result<Board>> CreateBoardAsync(string name, string? description, CancellationToken cancellationToken = default)
    {
        var board = await trelloClient.CreateBoardAsync(name, description, cancellationToken);

        if (board is null)
        {
            return Result.Fail<Board>(new EntityDoesNotExistError());
        }

        return Result.Ok(board);
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

    public async Task<Result<Board>> ArchiveBoardAsync(string boardId, CancellationToken cancellationToken = default)
    {
        var board = await trelloClient.ArchiveBoardAsync(boardId, cancellationToken);

        if (board is null)
        {
            return Result.Fail<Board>(new EntityDoesNotExistError());
        }

        return Result.Ok(board);
    }

    public async Task<Result> DeleteBoardAsync(string boardId, CancellationToken cancellationToken = default)
    {
        var success = await trelloClient.DeleteBoardAsync(boardId, cancellationToken);

        if (!success)
        {
            return Result.Fail(new EntityDoesNotExistError());
        }

        return Result.Ok();
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

    public async Task<Result<Card>> CreateCardAsync(string listId, string name, string? description, CancellationToken cancellationToken = default)
    {
        var card = await trelloClient.CreateCardAsync(listId, name, description, cancellationToken);

        if (card is null)
        {
            return Result.Fail<Card>(new EntityDoesNotExistError());
        }

        return Result.Ok(card);
    }

    public async Task<Result<Card>> UpdateCardAsync(string cardId, string? name, string? description, CancellationToken cancellationToken = default)
    {
        var card = await trelloClient.UpdateCardAsync(cardId, name, description, cancellationToken);

        if (card is null)
        {
            return Result.Fail<Card>(new EntityDoesNotExistError());
        }

        return Result.Ok(card);
    }

    public async Task<Result<Card>> ArchiveCardAsync(string cardId, CancellationToken cancellationToken = default)
    {
        var card = await trelloClient.ArchiveCardAsync(cardId, cancellationToken);

        if (card is null)
        {
            return Result.Fail<Card>(new EntityDoesNotExistError());
        }

        return Result.Ok(card);
    }

    public async Task<Result<Card>> AddLabelToCardAsync(string cardId, string labelId, CancellationToken cancellationToken = default)
    {
        var card = await trelloClient.AddLabelToCardAsync(cardId, labelId, cancellationToken);

        if (card is null)
        {
            return Result.Fail<Card>(new EntityDoesNotExistError());
        }

        return Result.Ok(card);
    }

    public async Task<Result<Card>> RemoveLabelFromCardAsync(string cardId, string labelId, CancellationToken cancellationToken = default)
    {
        var card = await trelloClient.RemoveLabelFromCardAsync(cardId, labelId, cancellationToken);

        if (card is null)
        {
            return Result.Fail<Card>(new EntityDoesNotExistError());
        }

        return Result.Ok(card);
    }

    public async Task<Result> DeleteCardAsync(string cardId, CancellationToken cancellationToken = default)
    {
        var success = await trelloClient.DeleteCardAsync(cardId, cancellationToken);

        if (!success)
        {
            return Result.Fail(new EntityDoesNotExistError());
        }

        return Result.Ok();
    }

    public async Task<Result<List<Label>>> GetBoardLabelsAsync(string boardId, CancellationToken cancellationToken = default)
    {
        var labels = await trelloClient.GetBoardLabelsAsync(boardId, cancellationToken);
        return Result.Ok(labels);
    }

    public async Task<Result<Label>> CreateBoardLabelAsync(string boardId, string name, string color, CancellationToken cancellationToken = default)
    {
        var label = await trelloClient.CreateBoardLabelAsync(boardId, name, color, cancellationToken);

        if (label is null)
        {
            return Result.Fail<Label>(new EntityDoesNotExistError());
        }

        return Result.Ok(label);
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
