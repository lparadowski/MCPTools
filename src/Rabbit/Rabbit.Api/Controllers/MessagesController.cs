using Asp.Versioning;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;
using Rabbit.Api.Requests;
using Rabbit.Application.Services;
using Shared.Api.Extensions;

namespace Rabbit.Api.Controllers;

[ApiController]
[ApiVersion(Versions.V1)]
[Route("api/v{version:apiVersion}/[controller]")]
public class MessagesController(IRabbitService rabbitService) : ControllerBase
{
    [HttpPost]
    public async Task<Results<Ok, BadRequest, NotFound, ProblemHttpResult>> PublishAsync(
        [FromBody] PublishMessageRequest request,
        CancellationToken cancellationToken)
    {
        var result = await rabbitService.PublishAsync(
            request.Exchange,
            request.RoutingKey,
            request.Body,
            request.ContentType,
            request.Headers,
            cancellationToken);

        return result.ToOkPostResult();
    }
}
