using Asp.Versioning;
using Jira.Api.Extensions;
using Jira.Api.Requests;
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
public class SprintsController(IJiraService jiraService) : ControllerBase
{
    [HttpGet("by-board/{boardId:int}")]
    public async Task<Results<Ok<List<SprintResponse>>, BadRequest, ProblemHttpResult>> GetSprintsAsync(
        int boardId, CancellationToken cancellationToken)
    {
        var result = await jiraService.GetSprintsAsync(boardId, cancellationToken);
        return result.ToGetResult<Sprint, SprintResponse>(s => s.Adapt<SprintResponse>());
    }

    [HttpPost("{sprintId:int}/issues")]
    public async Task<Results<Ok, BadRequest, NotFound, ProblemHttpResult>> MoveIssuesToSprintAsync(
        int sprintId, [FromBody] MoveIssuesToSprintRequest request, CancellationToken cancellationToken)
    {
        var result = await jiraService.MoveIssuesToSprintAsync(sprintId, request.IssueKeys, cancellationToken);
        return result.ToOkPostResult();
    }
}
