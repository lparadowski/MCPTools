using Asp.Versioning;
using Shared.Api.Extensions;
using Polarion.Api.Responses;
using Polarion.Application.Interfaces;
using Polarion.Domain.Entities;
using Mapster;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;

namespace Polarion.Api.Controllers;

[ApiController]
[ApiVersion(Versions.V1)]
[Route("api/v{version:apiVersion}/[controller]")]
public class ProjectsController(IPolarionService polarionService) : ControllerBase
{
    [HttpGet]
    public async Task<Results<Ok<List<ProjectResponse>>, BadRequest, ProblemHttpResult>> GetProjectsAsync(
        CancellationToken cancellationToken)
    {
        var result = await polarionService.GetProjectsAsync(cancellationToken);
        return result.ToGetResult<Project, ProjectResponse>(p => p.Adapt<ProjectResponse>());
    }

    [HttpGet("{projectId}")]
    public async Task<Results<Ok<ProjectResponse>, BadRequest, NotFound, ProblemHttpResult>> GetProjectAsync(
        string projectId, CancellationToken cancellationToken)
    {
        var result = await polarionService.GetProjectAsync(projectId, cancellationToken);
        return result.ToGetResult<Project, ProjectResponse>(p => p.Adapt<ProjectResponse>());
    }
}
