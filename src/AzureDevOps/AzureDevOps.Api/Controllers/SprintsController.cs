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
[Route("api/v{version:apiVersion}/projects/{project}/teams/{team}/[controller]")]
public class SprintsController(IAzureDevOpsService azureDevOpsService) : ControllerBase
{
    [HttpGet]
    public async Task<Results<Ok<List<SprintResponse>>, BadRequest, ProblemHttpResult>> GetSprintsAsync(
        string project, string team, CancellationToken cancellationToken)
    {
        var result = await azureDevOpsService.GetSprintsAsync(project, team, cancellationToken);
        return result.ToGetResult<Sprint, SprintResponse>(s => s.Adapt<SprintResponse>());
    }
}
