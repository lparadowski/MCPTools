using Asp.Versioning;
using Shared.Api.Extensions;
using AzureDevOps.Api.Responses;
using AzureDevOps.Application.Interfaces;
using AzureDevOps.Domain.Entities;
using Mapster;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;

namespace AzureDevOps.Api.Controllers;

[ApiController]
[ApiVersion(Versions.V1)]
[Route("api/v{version:apiVersion}/[controller]")]
public class ProjectsController(IAzureDevOpsService azureDevOpsService) : ControllerBase
{
    [HttpGet]
    public async Task<Results<Ok<List<ProjectResponse>>, BadRequest, ProblemHttpResult>> GetProjectsAsync(
        CancellationToken cancellationToken)
    {
        var result = await azureDevOpsService.GetProjectsAsync(cancellationToken);
        return result.ToGetResult<Project, ProjectResponse>(p => p.Adapt<ProjectResponse>());
    }

    [HttpGet("{projectNameOrId}")]
    public async Task<Results<Ok<ProjectResponse>, BadRequest, NotFound, ProblemHttpResult>> GetProjectAsync(
        string projectNameOrId, CancellationToken cancellationToken)
    {
        var result = await azureDevOpsService.GetProjectAsync(projectNameOrId, cancellationToken);
        return result.ToGetResult<Project, ProjectResponse>(p => p.Adapt<ProjectResponse>());
    }
}
