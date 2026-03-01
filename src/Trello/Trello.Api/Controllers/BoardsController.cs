using Asp.Versioning;
using Mapster;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;
using Trello.Api.Extensions;
using Trello.Api.Requests;
using Trello.Api.Responses;
using Trello.Application.Services;
using Trello.Domain.Entities;

namespace Trello.Api.Controllers;

[ApiController]
[ApiVersion(Versions.V1)]
[Route("api/v{version:apiVersion}/[controller]")]
public class BoardsController(ITrelloService trelloService) : ControllerBase
{
    [HttpGet]
    public async Task<Results<Ok<List<BoardResponse>>, BadRequest, ProblemHttpResult>> GetBoardsAsync(
        CancellationToken cancellationToken)
    {
        var result = await trelloService.GetBoardsAsync(cancellationToken);
        return result.ToGetResult<Board, BoardResponse>(b => b.Adapt<BoardResponse>());
    }

    [HttpGet("{id}")]
    public async Task<Results<Ok<BoardResponse>, BadRequest, NotFound, ProblemHttpResult>> GetBoardByIdAsync(
        string id, CancellationToken cancellationToken)
    {
        var result = await trelloService.GetBoardByIdAsync(id, cancellationToken);
        return result.ToGetResult<Board, BoardResponse>(b => b.Adapt<BoardResponse>());
    }

    [HttpGet("{id}/cards")]
    public async Task<Results<Ok<List<CardResponse>>, BadRequest, ProblemHttpResult>> GetCardsByBoardIdAsync(
        string id, CancellationToken cancellationToken)
    {
        var result = await trelloService.GetCardsByBoardIdAsync(id, cancellationToken);
        return result.ToGetResult<Card, CardResponse>(c => c.Adapt<CardResponse>());
    }

    [HttpGet("{id}/labels")]
    public async Task<Results<Ok<List<LabelResponse>>, BadRequest, ProblemHttpResult>> GetBoardLabelsAsync(
        string id, CancellationToken cancellationToken)
    {
        var result = await trelloService.GetBoardLabelsAsync(id, cancellationToken);
        return result.ToGetResult<Label, LabelResponse>(l => l.Adapt<LabelResponse>());
    }

    [HttpPost("{id}/labels")]
    public async Task<Results<Ok<LabelResponse>, BadRequest, NotFound, ProblemHttpResult>> CreateBoardLabelAsync(
        string id, [FromBody] CreateLabelRequest request, CancellationToken cancellationToken)
    {
        var result = await trelloService.CreateBoardLabelAsync(id, request.Name, request.Color, cancellationToken);
        return result.ToPutResult<Label, LabelResponse>(l => l.Adapt<LabelResponse>());
    }
}
