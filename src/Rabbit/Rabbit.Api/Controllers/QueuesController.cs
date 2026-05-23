using Asp.Versioning;
using Mapster;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;
using Rabbit.Api.Responses;
using Rabbit.Application.Services;
using Rabbit.Domain.Entities;
using Shared.Api.Extensions;

namespace Rabbit.Api.Controllers;

[ApiController]
[ApiVersion(Versions.V1)]
[Route("api/v{version:apiVersion}/[controller]")]
public class QueuesController(IRabbitService rabbitService) : ControllerBase
{
    [HttpGet]
    public async Task<Results<Ok<List<QueueResponse>>, BadRequest, ProblemHttpResult>> ListAsync(
        CancellationToken cancellationToken)
    {
        var result = await rabbitService.ListQueuesAsync(cancellationToken);
        return result.ToGetResult<Queue, QueueResponse>(q => q.Adapt<QueueResponse>());
    }

    [HttpPost("{queueName}/peek")]
    public async Task<Results<Ok<List<MessageResponse>>, BadRequest, ProblemHttpResult>> PeekAsync(
        string queueName,
        [FromQuery] int count = 5,
        CancellationToken cancellationToken = default)
    {
        var result = await rabbitService.PeekAsync(queueName, count, cancellationToken);
        return result.ToGetResult<Message, MessageResponse>(m => m.Adapt<MessageResponse>());
    }
}
