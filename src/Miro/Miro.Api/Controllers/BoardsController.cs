using Asp.Versioning;
using Mapster;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;
using Miro.Api.Extensions;
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
}
