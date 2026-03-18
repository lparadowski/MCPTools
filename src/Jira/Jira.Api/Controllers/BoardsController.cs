using Asp.Versioning;
using Jira.Api.Extensions;
using Jira.Api.Responses;
using Jira.Application.Interfaces;
using Jira.Domain.Entities;
using Mapster;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;

namespace Jira.Api.Controllers;

[ApiController]
[ApiVersion(Versions.V1)]
[Route("api/v{version:apiVersion}/[controller]")]
public class BoardsController(IJiraService jiraService) : ControllerBase
{
    [HttpGet]
    public async Task<Results<Ok<List<BoardResponse>>, BadRequest, ProblemHttpResult>> GetBoardsAsync(
        CancellationToken cancellationToken)
    {
        var result = await jiraService.GetBoardsAsync(cancellationToken);
        return result.ToGetResult<Board, BoardResponse>(b => b.Adapt<BoardResponse>());
    }

    [HttpGet("{boardId:int}")]
    public async Task<Results<Ok<BoardResponse>, BadRequest, NotFound, ProblemHttpResult>> GetBoardAsync(
        int boardId, CancellationToken cancellationToken)
    {
        var result = await jiraService.GetBoardAsync(boardId, cancellationToken);
        return result.ToGetResult<Board, BoardResponse>(b => b.Adapt<BoardResponse>());
    }
}
