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
[Route("api/v{version:apiVersion}/repositories/{owner}/{repo}/[controller]")]
public class PullRequestsController(IGitHubService gitHubService) : ControllerBase
{
    [HttpGet]
    public async Task<Results<Ok<List<PullRequestResponse>>, BadRequest, ProblemHttpResult>> GetPullRequestsAsync(
        string owner, string repo, [FromQuery] string? state = null, [FromQuery] int maxResults = 30,
        CancellationToken cancellationToken = default)
    {
        var result = await gitHubService.GetPullRequestsAsync(owner, repo, state, maxResults, cancellationToken);
        return result.ToGetResult<PullRequest, PullRequestResponse>(pr => pr.Adapt<PullRequestResponse>());
    }

    [HttpGet("{number:int}")]
    public async Task<Results<Ok<PullRequestResponse>, BadRequest, NotFound, ProblemHttpResult>> GetPullRequestAsync(
        string owner, string repo, int number, CancellationToken cancellationToken)
    {
        var result = await gitHubService.GetPullRequestAsync(owner, repo, number, cancellationToken);
        return result.ToGetResult<PullRequest, PullRequestResponse>(pr => pr.Adapt<PullRequestResponse>());
    }

    [HttpGet("{number:int}/reviews")]
    public async Task<Results<Ok<List<ReviewResponse>>, BadRequest, ProblemHttpResult>> GetPullRequestReviewsAsync(
        string owner, string repo, int number, CancellationToken cancellationToken)
    {
        var result = await gitHubService.GetPullRequestReviewsAsync(owner, repo, number, cancellationToken);
        return result.ToGetResult<Review, ReviewResponse>(r => r.Adapt<ReviewResponse>());
    }

    [HttpGet("{number:int}/reviewcomments")]
    public async Task<Results<Ok<List<ReviewCommentResponse>>, BadRequest, ProblemHttpResult>> GetPullRequestReviewCommentsAsync(
        string owner, string repo, int number, CancellationToken cancellationToken)
    {
        var result = await gitHubService.GetPullRequestReviewCommentsAsync(owner, repo, number, cancellationToken);
        return result.ToGetResult<ReviewComment, ReviewCommentResponse>(rc => rc.Adapt<ReviewCommentResponse>());
    }
}
