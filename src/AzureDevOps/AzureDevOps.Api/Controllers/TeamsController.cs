using Asp.Versioning;
using AzureDevOps.Api.Extensions;
using AzureDevOps.Api.Responses;
using AzureDevOps.Application.Interfaces;
using AzureDevOps.Domain.Entities;
using Mapster;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;

namespace AzureDevOps.Api.Controllers;

[ApiController]
[ApiVersion(Versions.V1)]
[Route("api/v{version:apiVersion}/projects/{project}/[controller]")]
public class TeamsController(IAzureDevOpsService azureDevOpsService) : ControllerBase
{
    [HttpGet]
    public async Task<Results<Ok<List<TeamResponse>>, BadRequest, ProblemHttpResult>> GetTeamsAsync(
        string project, CancellationToken cancellationToken)
    {
        var result = await azureDevOpsService.GetTeamsAsync(project, cancellationToken);
        return result.ToGetResult<Team, TeamResponse>(t => t.Adapt<TeamResponse>());
    }
}
