using Asp.Versioning;
using Shared.Api.Extensions;
using Confluence.Api.Responses;
using Confluence.Application.Services;
using Confluence.Domain.Entities;
using Mapster;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;

namespace Confluence.Api.Controllers;

[ApiController]
[ApiVersion(Versions.V1)]
[Route("api/v{version:apiVersion}/[controller]")]
public class ActivityController(IConfluenceService confluenceService) : ControllerBase
{
    [HttpGet]
    public async Task<Results<Ok<List<ActivityItemResponse>>, BadRequest, ProblemHttpResult>> GetUserActivityAsync(
        [FromQuery] string accountId, [FromQuery] DateTime startDate, [FromQuery] DateTime endDate,
        CancellationToken cancellationToken = default)
    {
        var result = await confluenceService.GetUserActivityAsync(accountId, startDate, endDate, cancellationToken);
        return result.ToGetResult<ActivityItem, ActivityItemResponse>(i => i.Adapt<ActivityItemResponse>());
    }
}
