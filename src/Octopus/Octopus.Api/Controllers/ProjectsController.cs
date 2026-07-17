using Asp.Versioning;
using Mapster;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;
using Octopus.Api.Responses;
using Octopus.Application.Interfaces;
using Octopus.Domain.Entities;
using Shared.Api.Extensions;

namespace Octopus.Api.Controllers;

[ApiController]
[ApiVersion(Versions.V1)]
[Route("api/v{version:apiVersion}/[controller]")]
public class ProjectsController(IOctopusService octopusService) : ControllerBase
{
    [HttpGet]
    public async Task<Results<Ok<List<ProjectResponse>>, BadRequest, ProblemHttpResult>> GetAsync(
        CancellationToken cancellationToken)
    {
        var result = await octopusService.GetProjectsAsync(cancellationToken);
        return result.ToGetResult<Project, ProjectResponse>(p => p.Adapt<ProjectResponse>());
    }
}
