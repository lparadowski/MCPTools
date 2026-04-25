using Asp.Versioning;
using Shared.Api.Extensions;
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
public class ProjectsController(IJiraService jiraService) : ControllerBase
{
    [HttpGet]
    public async Task<Results<Ok<List<ProjectResponse>>, BadRequest, ProblemHttpResult>> GetProjectsAsync(
        CancellationToken cancellationToken)
    {
        var result = await jiraService.GetProjectsAsync(cancellationToken);
        return result.ToGetResult<Project, ProjectResponse>(p => p.Adapt<ProjectResponse>());
    }

    [HttpGet("{projectKeyOrId}")]
    public async Task<Results<Ok<ProjectResponse>, BadRequest, NotFound, ProblemHttpResult>> GetProjectAsync(
        string projectKeyOrId, CancellationToken cancellationToken)
    {
        var result = await jiraService.GetProjectAsync(projectKeyOrId, cancellationToken);
        return result.ToGetResult<Project, ProjectResponse>(p => p.Adapt<ProjectResponse>());
    }
}
