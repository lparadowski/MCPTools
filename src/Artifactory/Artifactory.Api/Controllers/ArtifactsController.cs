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
public class ArtifactsController(IArtifactoryService artifactoryService) : ControllerBase
{
    [HttpGet("search")]
    public async Task<Results<Ok<List<ArtifactItemResponse>>, BadRequest, ProblemHttpResult>> SearchAsync(
        [FromQuery] string name,
        [FromQuery] string? repo = null,
        CancellationToken cancellationToken = default)
    {
        var result = await artifactoryService.SearchArtifactsAsync(name, repo, cancellationToken);
        return result.ToGetResult<ArtifactItem, ArtifactItemResponse>(a => a.Adapt<ArtifactItemResponse>());
    }
}
