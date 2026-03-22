using Asp.Versioning;
using Mapster;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;
using Miro.Api.Extensions;
using Miro.Api.Requests;
using Miro.Api.Responses;
using Miro.Application.Services;
using Miro.Domain.Entities;

namespace Miro.Api.Controllers;

[ApiController]
[ApiVersion(Versions.V1)]
[Route("api/v{version:apiVersion}/[controller]")]
public class BoardsController(IMiroService miroService) : ControllerBase
{
    [HttpGet]
    public async Task<Results<Ok<List<BoardResponse>>, BadRequest, ProblemHttpResult>> GetBoardsAsync(
        CancellationToken cancellationToken)
    {
        var result = await miroService.GetBoardsAsync(cancellationToken);
        return result.ToGetResult<Board, BoardResponse>(b => b.Adapt<BoardResponse>());
    }

    [HttpGet("{id}")]
    public async Task<Results<Ok<BoardResponse>, BadRequest, NotFound, ProblemHttpResult>> GetBoardByIdAsync(
        string id, CancellationToken cancellationToken)
    {
        var result = await miroService.GetBoardByIdAsync(id, cancellationToken);
        return result.ToGetResult<Board, BoardResponse>(b => b.Adapt<BoardResponse>());
    }

    [HttpGet("{id}/sticky-notes")]
    public async Task<Results<Ok<List<StickyNoteResponse>>, BadRequest, ProblemHttpResult>> GetStickyNotesByBoardIdAsync(
        string id, CancellationToken cancellationToken)
    {
        var result = await miroService.GetStickyNotesByBoardIdAsync(id, cancellationToken);
        return result.ToGetResult<StickyNote, StickyNoteResponse>(s => s.Adapt<StickyNoteResponse>());
    }

    [HttpPost("{id}/sticky-notes")]
    public async Task<Results<Ok<StickyNoteResponse>, BadRequest, NotFound, ProblemHttpResult>> CreateStickyNoteAsync(
        string id, [FromBody] CreateStickyNoteRequest request, CancellationToken cancellationToken)
    {
        var result = await miroService.CreateStickyNoteAsync(
            id, request.Content, request.Shape, request.FillColor,
            request.PositionX, request.PositionY, cancellationToken);
        return result.ToGetResult<StickyNote, StickyNoteResponse>(s => s.Adapt<StickyNoteResponse>());
    }

    [HttpPatch("{id}/sticky-notes/{itemId}")]
    public async Task<Results<Ok<StickyNoteResponse>, BadRequest, NotFound, ProblemHttpResult>> UpdateStickyNoteAsync(
        string id, string itemId, [FromBody] UpdateStickyNoteRequest request, CancellationToken cancellationToken)
    {
        var result = await miroService.UpdateStickyNoteAsync(
            id, itemId, request.Content, request.FillColor,
            request.PositionX, request.PositionY, cancellationToken);
        return result.ToGetResult<StickyNote, StickyNoteResponse>(s => s.Adapt<StickyNoteResponse>());
    }

    [HttpDelete("{id}/sticky-notes/{itemId}")]
    public async Task<Results<Ok, ProblemHttpResult>> DeleteStickyNoteAsync(
        string id, string itemId, CancellationToken cancellationToken)
    {
        var result = await miroService.DeleteStickyNoteAsync(id, itemId, cancellationToken);

        if (result.IsSuccess)
        {
            return TypedResults.Ok();
        }

        return TypedResults.Problem(statusCode: StatusCodes.Status500InternalServerError);
    }
}
