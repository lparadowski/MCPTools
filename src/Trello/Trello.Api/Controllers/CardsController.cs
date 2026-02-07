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
public class CardsController(ITrelloService trelloService) : ControllerBase
{
    [HttpGet("{id}")]
    public async Task<Results<Ok<CardResponse>, BadRequest, NotFound, ProblemHttpResult>> GetCardByIdAsync(
        string id, CancellationToken cancellationToken)
    {
        var result = await trelloService.GetCardByIdAsync(id, cancellationToken);
        return result.ToGetResult<Card, CardResponse>(c => c.Adapt<CardResponse>());
    }

    [HttpGet("{id}/comments")]
    public async Task<Results<Ok<List<CommentResponse>>, BadRequest, ProblemHttpResult>> GetCommentsByCardIdAsync(
        string id, CancellationToken cancellationToken)
    {
        var result = await trelloService.GetCommentsByCardIdAsync(id, cancellationToken);
        return result.ToGetResult<Comment, CommentResponse>(c => c.Adapt<CommentResponse>());
    }

    [HttpPost("{id}/comments")]
    public async Task<Results<Ok, BadRequest, NotFound, ProblemHttpResult>> PostCommentAsync(
        string id, [FromBody] PostCommentRequest request, CancellationToken cancellationToken)
    {
        var result = await trelloService.AddCommentAsync(id, request.Text, cancellationToken);
        return result.ToOkPostResult();
    }

    [HttpPut("{id}/list")]
    public async Task<Results<Ok<CardResponse>, BadRequest, NotFound, ProblemHttpResult>> MoveCardToListAsync(
        string id, [FromBody] MoveCardRequest request, CancellationToken cancellationToken)
    {
        var result = await trelloService.MoveCardToListAsync(id, request.ListId, cancellationToken);
        return result.ToPutResult<Card, CardResponse>(c => c.Adapt<CardResponse>());
    }
}
