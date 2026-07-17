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
public class DeploymentsController(IOctopusService octopusService) : ControllerBase
{
    [HttpGet("deployed")]
    public async Task<Results<Ok<List<DeployedReleaseResponse>>, BadRequest, ProblemHttpResult>> GetDeployedAsync(
        [FromQuery] string environment,
        [FromQuery] DateTimeOffset? asOf = null,
        [FromQuery] string? project = null,
        CancellationToken cancellationToken = default)
    {
        var result = await octopusService.GetDeployedReleasesAsync(environment, asOf, project, cancellationToken);
        return result.ToGetResult<DeployedRelease, DeployedReleaseResponse>(r => r.Adapt<DeployedReleaseResponse>());
    }
}
