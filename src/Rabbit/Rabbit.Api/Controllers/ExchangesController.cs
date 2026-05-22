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
public class ExchangesController(IRabbitService rabbitService) : ControllerBase
{
    [HttpGet]
    public async Task<Results<Ok<List<ExchangeResponse>>, BadRequest, ProblemHttpResult>> ListAsync(
        CancellationToken cancellationToken)
    {
        var result = await rabbitService.ListExchangesAsync(cancellationToken);
        return result.ToGetResult<Exchange, ExchangeResponse>(e => e.Adapt<ExchangeResponse>());
    }
}
