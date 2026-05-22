using Asp.Versioning;
using Mapster;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;
using Rabbit.Api.Requests;
using Rabbit.Api.Responses;
using Rabbit.Application.Services;
using Rabbit.Domain.Entities;
using Shared.Api.Extensions;

namespace Rabbit.Api.Controllers;

[ApiController]
[ApiVersion(Versions.V1)]
[Route("api/v{version:apiVersion}/[controller]")]
public class SubscriptionsController(IRabbitService rabbitService) : ControllerBase
{
    [HttpGet]
    public async Task<Results<Ok<List<SubscriptionResponse>>, BadRequest, ProblemHttpResult>> ListAsync(
        CancellationToken cancellationToken)
    {
        var result = await rabbitService.ListSubscriptionsAsync(cancellationToken);
        return result.ToGetResult<Subscription, SubscriptionResponse>(s => s.Adapt<SubscriptionResponse>());
    }

    [HttpPost]
    public async Task<Results<Ok<SubscriptionResponse>, BadRequest, NotFound, ProblemHttpResult>> SubscribeAsync(
        [FromBody] CreateSubscriptionRequest request,
        CancellationToken cancellationToken)
    {
        var result = await rabbitService.SubscribeAsync(request.Exchange, request.RoutingKey, request.QueueName, cancellationToken);
        return result.ToGetResult<Subscription, SubscriptionResponse>(s => s.Adapt<SubscriptionResponse>());
    }

    [HttpPost("{subscriptionId}/drain")]
    public async Task<Results<Ok<List<MessageResponse>>, BadRequest, ProblemHttpResult>> DrainAsync(
        string subscriptionId,
        [FromQuery] int maxMessages = 10,
        [FromQuery] int waitMs = 2000,
        CancellationToken cancellationToken = default)
    {
        var result = await rabbitService.DrainAsync(subscriptionId, maxMessages, waitMs, cancellationToken);
        return result.ToGetResult<Message, MessageResponse>(m => m.Adapt<MessageResponse>());
    }

    [HttpDelete("{subscriptionId}")]
    public async Task<Results<Ok, BadRequest, NotFound, ProblemHttpResult>> UnsubscribeAsync(
        string subscriptionId,
        CancellationToken cancellationToken)
    {
        var result = await rabbitService.UnsubscribeAsync(subscriptionId, cancellationToken);
        return result.ToOkPostResult();
    }
}
