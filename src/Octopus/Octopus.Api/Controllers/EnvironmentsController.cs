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
public class EnvironmentsController(IOctopusService octopusService) : ControllerBase
{
    [HttpGet]
    public async Task<Results<Ok<List<EnvironmentResponse>>, BadRequest, ProblemHttpResult>> GetAsync(
        CancellationToken cancellationToken)
    {
        var result = await octopusService.GetEnvironmentsAsync(cancellationToken);
        return result.ToGetResult<DeploymentEnvironment, EnvironmentResponse>(e => e.Adapt<EnvironmentResponse>());
    }
}
