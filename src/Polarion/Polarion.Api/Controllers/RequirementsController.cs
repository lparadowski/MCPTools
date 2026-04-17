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
[Route("api/v{version:apiVersion}/projects/{projectId}/[controller]")]
public class RequirementsController(IPolarionService polarionService) : ControllerBase
{
    [HttpGet]
    public async Task<Results<Ok<List<RequirementResponse>>, BadRequest, ProblemHttpResult>> GetRequirementsAsync(
        string projectId, [FromQuery] string? query = null, [FromQuery] int maxResults = 50,
        CancellationToken cancellationToken = default)
    {
        var result = await polarionService.GetRequirementsAsync(projectId, query, maxResults, cancellationToken);
        return result.ToGetResult<Requirement, RequirementResponse>(r => r.Adapt<RequirementResponse>());
    }

    [HttpGet("documents/{spaceId}/{documentName}")]
    public async Task<Results<Ok<List<RequirementResponse>>, BadRequest, ProblemHttpResult>> GetDocumentWorkItemsAsync(
        string projectId, string spaceId, string documentName, [FromQuery] int maxResults = 50,
        CancellationToken cancellationToken = default)
    {
        var result = await polarionService.GetDocumentWorkItemsAsync(projectId, spaceId, documentName, maxResults, cancellationToken);
        return result.ToGetResult<Requirement, RequirementResponse>(r => r.Adapt<RequirementResponse>());
    }

    [HttpGet("{workItemId}")]
    public async Task<Results<Ok<RequirementResponse>, BadRequest, NotFound, ProblemHttpResult>> GetRequirementAsync(
        string projectId, string workItemId, CancellationToken cancellationToken)
    {
        var result = await polarionService.GetRequirementAsync(projectId, workItemId, cancellationToken);
        return result.ToGetResult<Requirement, RequirementResponse>(r => r.Adapt<RequirementResponse>());
    }

    [HttpGet("{workItemId}/links")]
    public async Task<Results<Ok<List<LinkedWorkItemResponse>>, BadRequest, ProblemHttpResult>> GetLinkedWorkItemsAsync(
        string projectId, string workItemId, CancellationToken cancellationToken)
    {
        var result = await polarionService.GetLinkedWorkItemsAsync(projectId, workItemId, cancellationToken);
        return result.ToGetResult<LinkedWorkItem, LinkedWorkItemResponse>(l => l.Adapt<LinkedWorkItemResponse>());
    }
}
