using Asp.Versioning;
using Mapster;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;
using Artifactory.Api.Responses;
using Artifactory.Application.Interfaces;
using Artifactory.Domain.Entities;
using Shared.Api.Extensions;

namespace Artifactory.Api.Controllers;

[ApiController]
[ApiVersion(Versions.V1)]
[Route("api/v{version:apiVersion}/[controller]")]
public class BuildsController(IArtifactoryService artifactoryService) : ControllerBase
{
    [HttpGet]
    public async Task<Results<Ok<List<BuildSummaryResponse>>, BadRequest, ProblemHttpResult>> GetAsync(
        [FromQuery] string? filter = null,
        CancellationToken cancellationToken = default)
    {
        var result = await artifactoryService.GetBuildsAsync(filter, cancellationToken);
        return result.ToGetResult<BuildSummary, BuildSummaryResponse>(b => b.Adapt<BuildSummaryResponse>());
    }

    [HttpGet("info")]
    public async Task<Results<Ok<BuildDetailResponse>, BadRequest, NotFound, ProblemHttpResult>> GetInfoAsync(
        [FromQuery] string name,
        [FromQuery] string? number = null,
        CancellationToken cancellationToken = default)
    {
        var result = await artifactoryService.GetBuildInfoAsync(name, number, cancellationToken);
        return result.ToGetResult<BuildDetail, BuildDetailResponse>(b => b.Adapt<BuildDetailResponse>());
    }
}
