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
public class WorklogsController(IJiraService jiraService) : ControllerBase
{
    [HttpGet]
    public async Task<Results<Ok<List<WorklogResponse>>, BadRequest, ProblemHttpResult>> GetUserWorklogsAsync(
        [FromQuery] string username,
        [FromQuery] DateTime startDate,
        [FromQuery] DateTime endDate,
        CancellationToken cancellationToken)
    {
        var result = await jiraService.GetUserWorklogsAsync(username, startDate, endDate, cancellationToken);
        return result.ToGetResult<Worklog, WorklogResponse>(w => w.Adapt<WorklogResponse>());
    }

    [HttpPut("{issueKeyOrId}/{worklogId}")]
    public async Task<Results<Ok<WorklogResponse>, BadRequest, NotFound, ProblemHttpResult>> UpdateWorklogAsync(
        string issueKeyOrId, string worklogId, [FromBody] AddWorklogRequest request, CancellationToken cancellationToken)
    {
        var result = await jiraService.UpdateWorklogAsync(issueKeyOrId, worklogId, request.TimeSpent, request.Comment, request.Started?.DateTime, cancellationToken);
        return result.ToPutResult<Worklog, WorklogResponse>(w => w.Adapt<WorklogResponse>());
    }

    [HttpDelete("{issueKeyOrId}/{worklogId}")]
    public async Task<Results<Ok, BadRequest, NotFound, ProblemHttpResult>> DeleteWorklogAsync(
        string issueKeyOrId, string worklogId, CancellationToken cancellationToken)
    {
        var result = await jiraService.DeleteWorklogAsync(issueKeyOrId, worklogId, cancellationToken);
        return result.ToOkPostResult();
    }
}
