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

    [HttpPost]
    public async Task<Results<Ok<CardResponse>, BadRequest, NotFound, ProblemHttpResult>> CreateCardAsync(
        [FromBody] CreateCardRequest request, CancellationToken cancellationToken)
    {
        var result = await trelloService.CreateCardAsync(request.ListId, request.Name, request.Description, cancellationToken);
        return result.ToPutResult<Card, CardResponse>(c => c.Adapt<CardResponse>());
    }

    [HttpPut("{id}/list")]
    public async Task<Results<Ok<CardResponse>, BadRequest, NotFound, ProblemHttpResult>> MoveCardToListAsync(
        string id, [FromBody] MoveCardRequest request, CancellationToken cancellationToken)
    {
        var result = await trelloService.MoveCardToListAsync(id, request.ListId, cancellationToken);
        return result.ToPutResult<Card, CardResponse>(c => c.Adapt<CardResponse>());
    }

    [HttpPut("{id}")]
    public async Task<Results<Ok<CardResponse>, BadRequest, NotFound, ProblemHttpResult>> UpdateCardAsync(
        string id, [FromBody] UpdateCardRequest request, CancellationToken cancellationToken)
    {
        var result = await trelloService.UpdateCardAsync(id, request.Name, request.Description, cancellationToken);
        return result.ToPutResult<Card, CardResponse>(c => c.Adapt<CardResponse>());
    }

    [HttpPut("{id}/archive")]
    public async Task<Results<Ok<CardResponse>, BadRequest, NotFound, ProblemHttpResult>> ArchiveCardAsync(
        string id, CancellationToken cancellationToken)
    {
        var result = await trelloService.ArchiveCardAsync(id, cancellationToken);
        return result.ToPutResult<Card, CardResponse>(c => c.Adapt<CardResponse>());
    }

    [HttpDelete("{id}")]
    public async Task<Results<Ok, BadRequest, NotFound, ProblemHttpResult>> DeleteCardAsync(
        string id, CancellationToken cancellationToken)
    {
        var result = await trelloService.DeleteCardAsync(id, cancellationToken);
        return result.ToOkPostResult();
    }

    [HttpPost("{id}/labels")]
    public async Task<Results<Ok<CardResponse>, BadRequest, NotFound, ProblemHttpResult>> AddLabelAsync(
        string id, [FromBody] AddLabelRequest request, CancellationToken cancellationToken)
    {
        var result = await trelloService.AddLabelToCardAsync(id, request.LabelId, cancellationToken);
        return result.ToPutResult<Card, CardResponse>(c => c.Adapt<CardResponse>());
    }

    [HttpDelete("{id}/labels/{labelId}")]
    public async Task<Results<Ok<CardResponse>, BadRequest, NotFound, ProblemHttpResult>> RemoveLabelAsync(
        string id, string labelId, CancellationToken cancellationToken)
    {
        var result = await trelloService.RemoveLabelFromCardAsync(id, labelId, cancellationToken);
        return result.ToPutResult<Card, CardResponse>(c => c.Adapt<CardResponse>());
    }
}
