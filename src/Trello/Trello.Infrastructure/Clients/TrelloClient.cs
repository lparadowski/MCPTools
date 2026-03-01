using Mapster;
using Trello.Application.Interfaces;
using Trello.Domain.Entities;
using IBoard = Manatee.Trello.IBoard;
using ICard = Manatee.Trello.ICard;
using TrelloBoard = Manatee.Trello.Board;
using TrelloCard = Manatee.Trello.Card;
using TrelloFactory = Manatee.Trello.TrelloFactory;
using TrelloLabel = Manatee.Trello.Label;
using TrelloList = Manatee.Trello.List;

namespace Trello.Infrastructure.Clients;

public class TrelloClient : ITrelloClient
{
    public async Task<List<Board>> GetBoardsAsync(CancellationToken cancellationToken = default)
    {
        var me = await new TrelloFactory().Me();
        await me.Boards.Refresh(ct: cancellationToken);

        return me.Boards
            .Where(b => !b.IsClosed.GetValueOrDefault())
            .Select(b => b.Adapt<Board>())
            .ToList();
    }

    public async Task<Board?> GetBoardByIdAsync(string boardId, CancellationToken cancellationToken = default)
    {
        var board = new TrelloBoard(boardId);
        await board.Refresh(ct: cancellationToken);

        if (board.Name is null)
        {
            return null;
        }

        await board.Lists.Refresh(ct: cancellationToken);

        var result = ((IBoard)board).Adapt<Board>();
        result.Lists = board.Lists.Select(l => l.Adapt<BoardList>()).ToList();

        return result;
    }

    public async Task<List<Card>> GetCardsByBoardIdAsync(string boardId, CancellationToken cancellationToken = default)
    {
        var board = new TrelloBoard(boardId);
        await board.Refresh(ct: cancellationToken);
        await board.Lists.Refresh(ct: cancellationToken);

        var cards = new List<Card>();

        foreach (var list in board.Lists)
        {
            await list.Cards.Refresh(ct: cancellationToken);

            foreach (var card in list.Cards)
            {
                await card.Refresh(ct: cancellationToken);
                var mapped = ((ICard)card).Adapt<Card>();
                mapped.Comments = card.Comments?.Select(c => c.Adapt<Comment>()).ToList() ?? [];
                cards.Add(mapped);
            }
        }

        return cards;
    }

    public async Task<Card?> GetCardByIdAsync(string cardId, CancellationToken cancellationToken = default)
    {
        var card = new TrelloCard(cardId);
        await card.Refresh(ct: cancellationToken);

        if (card.Name is null)
        {
            return null;
        }

        await card.Comments.Refresh(ct: cancellationToken);

        var mapped = ((ICard)card).Adapt<Card>();
        mapped.Comments = card.Comments.Select(c => c.Adapt<Comment>()).ToList();

        return mapped;
    }

    public async Task<List<Comment>> GetCommentsByCardIdAsync(string cardId, CancellationToken cancellationToken = default)
    {
        var card = new TrelloCard(cardId);
        await card.Refresh(ct: cancellationToken);
        await card.Comments.Refresh(ct: cancellationToken);

        return card.Comments.Select(c => c.Adapt<Comment>()).ToList();
    }

    public async Task AddCommentAsync(string cardId, string text, CancellationToken cancellationToken = default)
    {
        var card = new TrelloCard(cardId);
        await card.Refresh(ct: cancellationToken);
        await card.Comments.Add(text, ct: cancellationToken);
    }

    public async Task<Card?> MoveCardToListAsync(string cardId, string listId, CancellationToken cancellationToken = default)
    {
        var card = new TrelloCard(cardId);
        await card.Refresh(ct: cancellationToken);

        if (card.Name is null)
        {
            return null;
        }

        var list = new TrelloList(listId);
        await list.Refresh(ct: cancellationToken);

        card.List = list;
        await card.Refresh(ct: cancellationToken);

        var mapped = ((ICard)card).Adapt<Card>();
        return mapped;
    }

    public async Task<Card?> CreateCardAsync(string listId, string name, string? description, CancellationToken cancellationToken = default)
    {
        var list = new TrelloList(listId);
        await list.Refresh(ct: cancellationToken);

        if (list.Name is null)
        {
            return null;
        }

        var card = await list.Cards.Add(name, description: description, ct: cancellationToken);

        await card.Refresh(ct: cancellationToken);

        return ((ICard)card).Adapt<Card>();
    }

    public async Task<Card?> UpdateCardAsync(string cardId, string? name, string? description, CancellationToken cancellationToken = default)
    {
        var card = new TrelloCard(cardId);
        await card.Refresh(ct: cancellationToken);

        if (card.Name is null)
        {
            return null;
        }

        if (name is not null)
        {
            card.Name = name;
        }

        if (description is not null)
        {
            card.Description = description;
        }

        await card.Refresh(ct: cancellationToken);

        return ((ICard)card).Adapt<Card>();
    }

    public async Task<Card?> ArchiveCardAsync(string cardId, CancellationToken cancellationToken = default)
    {
        var card = new TrelloCard(cardId);
        await card.Refresh(ct: cancellationToken);

        if (card.Name is null)
        {
            return null;
        }

        card.IsArchived = true;
        await card.Refresh(ct: cancellationToken);

        return ((ICard)card).Adapt<Card>();
    }

    public async Task<Card?> AddLabelToCardAsync(string cardId, string labelId, CancellationToken cancellationToken = default)
    {
        var card = new TrelloCard(cardId);
        await card.Refresh(ct: cancellationToken);

        if (card.Name is null)
        {
            return null;
        }

        await card.Board.Labels.Refresh(ct: cancellationToken);
        var label = card.Board.Labels.FirstOrDefault(l => l.Id == labelId);

        if (label is not null)
        {
            await card.Labels.Add(label, ct: cancellationToken);
        }

        await card.Refresh(ct: cancellationToken);

        return ((ICard)card).Adapt<Card>();
    }

    public async Task<Card?> RemoveLabelFromCardAsync(string cardId, string labelId, CancellationToken cancellationToken = default)
    {
        var card = new TrelloCard(cardId);
        await card.Refresh(ct: cancellationToken);

        if (card.Name is null)
        {
            return null;
        }

        await card.Labels.Refresh(ct: cancellationToken);
        var label = card.Labels.FirstOrDefault(l => l.Id == labelId);

        if (label is not null)
        {
            await card.Labels.Remove(label, ct: cancellationToken);
        }

        await card.Refresh(ct: cancellationToken);

        return ((ICard)card).Adapt<Card>();
    }

    public async Task<bool> DeleteCardAsync(string cardId, CancellationToken cancellationToken = default)
    {
        var card = new TrelloCard(cardId);
        await card.Refresh(ct: cancellationToken);

        if (card.Name is null)
        {
            return false;
        }

        await card.Delete(ct: cancellationToken);
        return true;
    }

    public async Task<List<Label>> GetBoardLabelsAsync(string boardId, CancellationToken cancellationToken = default)
    {
        var board = new TrelloBoard(boardId);
        await board.Refresh(ct: cancellationToken);
        await board.Labels.Refresh(ct: cancellationToken);

        return board.Labels
            .Select(l => l.Adapt<Label>())
            .ToList();
    }

    public async Task<Label?> CreateBoardLabelAsync(string boardId, string name, string color, CancellationToken cancellationToken = default)
    {
        var board = new TrelloBoard(boardId);
        await board.Refresh(ct: cancellationToken);

        if (board.Name is null)
        {
            return null;
        }

        var parsedColor = Enum.TryParse<Manatee.Trello.LabelColor>(color, ignoreCase: true, out var labelColor)
            ? labelColor
            : Manatee.Trello.LabelColor.Blue;

        var label = await board.Labels.Add(name, parsedColor, ct: cancellationToken);

        return label.Adapt<Label>();
    }
}
