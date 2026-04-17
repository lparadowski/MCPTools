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
public class RepositoriesController(IGitHubService gitHubService) : ControllerBase
{
    [HttpGet]
    public async Task<Results<Ok<List<RepositoryResponse>>, BadRequest, ProblemHttpResult>> GetRepositoriesAsync(
        CancellationToken cancellationToken)
    {
        var result = await gitHubService.GetRepositoriesAsync(cancellationToken);
        return result.ToGetResult<Repository, RepositoryResponse>(r => r.Adapt<RepositoryResponse>());
    }

    [HttpGet("{owner}/{repo}")]
    public async Task<Results<Ok<RepositoryResponse>, BadRequest, NotFound, ProblemHttpResult>> GetRepositoryAsync(
        string owner, string repo, CancellationToken cancellationToken)
    {
        var result = await gitHubService.GetRepositoryAsync(owner, repo, cancellationToken);
        return result.ToGetResult<Repository, RepositoryResponse>(r => r.Adapt<RepositoryResponse>());
    }
}
