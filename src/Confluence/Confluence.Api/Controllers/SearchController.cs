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
public class SearchController(IConfluenceService confluenceService) : ControllerBase
{
    [HttpPost]
    public async Task<Results<Ok<List<SearchResultResponse>>, BadRequest, ProblemHttpResult>> SearchAsync(
        [FromBody] SearchRequest request, CancellationToken cancellationToken)
    {
        var result = await confluenceService.SearchAsync(request.Cql, request.MaxResults, cancellationToken);
        return result.ToGetResult<SearchResult, SearchResultResponse>(r => r.Adapt<SearchResultResponse>());
    }
}
