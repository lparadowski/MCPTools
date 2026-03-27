using Asp.Versioning;
using Jira.Api.Extensions;
using Jira.Api.Responses;
using Jira.Application.Interfaces;
using Jira.Domain.Entities;
using Mapster;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;

namespace Jira.Api.Controllers;

[ApiController]
[ApiVersion(Versions.V1)]
[Route("api/v{version:apiVersion}/[controller]")]
public class ActivityController(IJiraService jiraService) : ControllerBase
{
    [HttpGet]
    public async Task<Results<Ok<List<UserActivityResponse>>, BadRequest, ProblemHttpResult>> GetUserActivityAsync(
        [FromQuery] string accountId,
        [FromQuery] DateTime startDate,
        [FromQuery] DateTime endDate,
        CancellationToken cancellationToken)
    {
        var result = await jiraService.GetUserActivityAsync(accountId, startDate, endDate, cancellationToken);
        return result.ToGetResult<UserActivity, UserActivityResponse>(a => a.Adapt<UserActivityResponse>());
    }
}
