using Asp.Versioning;
using GitHub.Api.Responses;
using GitHub.Application.Interfaces;
using GitHub.Domain.Entities;
using Mapster;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;
using Shared.Api.Extensions;

namespace GitHub.Api.Controllers;

[ApiController]
[ApiVersion(Versions.V1)]
[Route("api/v{version:apiVersion}/[controller]")]
public class ActivityController(IGitHubService gitHubService) : ControllerBase
{
    [HttpGet]
    public async Task<Results<Ok<List<ActivityEventResponse>>, BadRequest, ProblemHttpResult>> GetUserActivityAsync(
        [FromQuery] string? username = null, [FromQuery] DateOnly? from = null, [FromQuery] DateOnly? to = null,
        CancellationToken cancellationToken = default)
    {
        var result = await gitHubService.GetUserActivityAsync(username, from, to, cancellationToken);
        return result.ToGetResult<ActivityEvent, ActivityEventResponse>(e => e.Adapt<ActivityEventResponse>());
    }
}
