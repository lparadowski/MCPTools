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
[Route("api/v{version:apiVersion}/repositories/{owner}/{repo}/issues/{number:int}/[controller]")]
public class IssueCommentsController(IGitHubService gitHubService) : ControllerBase
{
    [HttpGet]
    public async Task<Results<Ok<List<IssueCommentResponse>>, BadRequest, ProblemHttpResult>> GetIssueCommentsAsync(
        string owner, string repo, int number, CancellationToken cancellationToken = default)
    {
        var result = await gitHubService.GetIssueCommentsAsync(owner, repo, number, cancellationToken);
        return result.ToGetResult<IssueComment, IssueCommentResponse>(c => c.Adapt<IssueCommentResponse>());
    }
}
