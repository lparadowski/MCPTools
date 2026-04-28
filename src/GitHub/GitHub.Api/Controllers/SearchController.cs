using Asp.Versioning;
using GitHub.Api.Responses;
using GitHub.Application.Interfaces;
using GitHub.Domain.Entities;
using Mapster;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;
using Shared.Api.Extensions;

namespace GitHub.Api.Controllers;

[ApiController]
[ApiVersion(Versions.V1)]
[Route("api/v{version:apiVersion}/[controller]")]
public class SearchController(IGitHubService gitHubService) : ControllerBase
{
    [HttpGet("pull-requests")]
    public async Task<Results<Ok<List<PullRequestResponse>>, BadRequest, ProblemHttpResult>> SearchPullRequestsAsync(
        [FromQuery] string query, CancellationToken cancellationToken = default)
    {
        var result = await gitHubService.SearchPullRequestsAsync(query, cancellationToken);
        return result.ToGetResult<PullRequest, PullRequestResponse>(pr => pr.Adapt<PullRequestResponse>());
    }
}
