using Asp.Versioning;
using Confluence.Api.Extensions;
using Confluence.Api.Requests;
using Confluence.Api.Responses;
using Confluence.Application.Services;
using Confluence.Domain.Entities;
using Mapster;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;

namespace Confluence.Api.Controllers;

[ApiController]
[ApiVersion(Versions.V1)]
[Route("api/v{version:apiVersion}/[controller]")]
public class SpacesController(IConfluenceService confluenceService) : ControllerBase
{
    [HttpGet]
    public async Task<Results<Ok<List<SpaceResponse>>, BadRequest, ProblemHttpResult>> GetSpacesAsync(
        CancellationToken cancellationToken)
    {
        var result = await confluenceService.GetSpacesAsync(cancellationToken);
        return result.ToGetResult<Space, SpaceResponse>(s => s.Adapt<SpaceResponse>());
    }

    [HttpGet("{id}")]
    public async Task<Results<Ok<SpaceResponse>, BadRequest, NotFound, ProblemHttpResult>> GetSpaceByIdAsync(
        string id, CancellationToken cancellationToken)
    {
        var result = await confluenceService.GetSpaceByIdAsync(id, cancellationToken);
        return result.ToGetResult<Space, SpaceResponse>(s => s.Adapt<SpaceResponse>());
    }

    [HttpPost]
    public async Task<Results<Ok<SpaceResponse>, BadRequest, NotFound, ProblemHttpResult>> CreateSpaceAsync(
        [FromBody] CreateSpaceRequest request, CancellationToken cancellationToken)
    {
        var result = await confluenceService.CreateSpaceAsync(request.Name, request.Key, request.Description, cancellationToken);
        return result.ToPutResult<Space, SpaceResponse>(s => s.Adapt<SpaceResponse>());
    }

    [HttpDelete("{id}")]
    public async Task<Results<Ok, BadRequest, NotFound, ProblemHttpResult>> DeleteSpaceAsync(
        string id, CancellationToken cancellationToken)
    {
        var result = await confluenceService.DeleteSpaceAsync(id, cancellationToken);
        return result.ToOkPostResult();
    }
}
