using Asp.Versioning;
using Shared.Api.Extensions;
using Confluence.Api.Requests;
using Confluence.Api.Responses;
using Confluence.Application.Services;
using Confluence.Domain.Entities;
using Mapster;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;
using Shared.Application.Chunking;

namespace Confluence.Api.Controllers;

[ApiController]
[ApiVersion(Versions.V1)]
[Route("api/v{version:apiVersion}/[controller]")]
public class PagesController(IConfluenceService confluenceService) : ControllerBase
{
    [HttpGet("by-space/{spaceId}")]
    public async Task<Results<Ok<List<PageResponse>>, BadRequest, ProblemHttpResult>> GetPagesBySpaceIdAsync(
        string spaceId, CancellationToken cancellationToken)
    {
        var result = await confluenceService.GetPagesBySpaceIdAsync(spaceId, cancellationToken);
        return result.ToGetResult<Page, PageResponse>(p => p.Adapt<PageResponse>());
    }

    [HttpGet("{id}")]
    public async Task<Results<Ok<PageResponse>, BadRequest, NotFound, ProblemHttpResult>> GetPageByIdAsync(
        string id, [FromQuery] int offset = 0, [FromQuery] int maxLength = 0,
        CancellationToken cancellationToken = default)
    {
        var result = await confluenceService.GetPageByIdAsync(id, offset, maxLength, cancellationToken);
        return result.ToGetResult<ChunkedResult<Page>, PageResponse>(chunkedResult =>
        {
            var response = chunkedResult.Value.Adapt<PageResponse>();

            if (chunkedResult.ChunkMetadata is not null)
            {
                response.TotalBodyLength = chunkedResult.ChunkMetadata.TotalLength;
                response.HasMore = chunkedResult.ChunkMetadata.HasMore;
                response.NextOffset = chunkedResult.ChunkMetadata.NextOffset;
            }

            return response;
        });
    }

    [HttpPost]
    public async Task<Results<Ok<PageResponse>, BadRequest, NotFound, ProblemHttpResult>> CreatePageAsync(
        [FromBody] CreatePageRequest request, CancellationToken cancellationToken)
    {
        var result = await confluenceService.CreatePageAsync(request.SpaceId, request.Title, request.Body, request.ParentId, cancellationToken);
        return result.ToPutResult<Page, PageResponse>(p => p.Adapt<PageResponse>());
    }

    [HttpPut("{id}")]
    public async Task<Results<Ok<PageResponse>, BadRequest, NotFound, ProblemHttpResult>> UpdatePageAsync(
        string id, [FromBody] UpdatePageRequest request, CancellationToken cancellationToken)
    {
        var result = await confluenceService.UpdatePageAsync(id, request.Title, request.Body, request.Version, cancellationToken);
        return result.ToPutResult<Page, PageResponse>(p => p.Adapt<PageResponse>());
    }

    [HttpDelete("{id}")]
    public async Task<Results<Ok, BadRequest, NotFound, ProblemHttpResult>> DeletePageAsync(
        string id, CancellationToken cancellationToken)
    {
        var result = await confluenceService.DeletePageAsync(id, cancellationToken);
        return result.ToOkPostResult();
    }
}
