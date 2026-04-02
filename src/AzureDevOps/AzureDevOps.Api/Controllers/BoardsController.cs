using Asp.Versioning;
using AzureDevOps.Api.Extensions;
using AzureDevOps.Api.Responses;
using AzureDevOps.Application.Interfaces;
using AzureDevOps.Domain.Entities;
using Mapster;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;

namespace AzureDevOps.Api.Controllers;

[ApiController]
[ApiVersion(Versions.V1)]
[Route("api/v{version:apiVersion}/projects/{project}/teams/{team}/[controller]")]
public class BoardsController(IAzureDevOpsService azureDevOpsService) : ControllerBase
{
    [HttpGet]
    public async Task<Results<Ok<List<BoardResponse>>, BadRequest, ProblemHttpResult>> GetBoardsAsync(
        string project, string team, CancellationToken cancellationToken)
    {
        var result = await azureDevOpsService.GetBoardsAsync(project, team, cancellationToken);
        return result.ToGetResult<Board, BoardResponse>(b => b.Adapt<BoardResponse>());
    }
}
