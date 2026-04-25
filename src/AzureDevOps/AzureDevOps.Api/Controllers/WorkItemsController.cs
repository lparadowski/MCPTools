using Asp.Versioning;
using Shared.Api.Extensions;
using AzureDevOps.Api.Requests;
using AzureDevOps.Api.Responses;
using AzureDevOps.Application.Interfaces;
using AzureDevOps.Domain.Entities;
using Mapster;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;

namespace AzureDevOps.Api.Controllers;

[ApiController]
[ApiVersion(Versions.V1)]
[Route("api/v{version:apiVersion}/[controller]")]
public class WorkItemsController(IAzureDevOpsService azureDevOpsService) : ControllerBase
{
    [HttpGet("{id:int}")]
    public async Task<Results<Ok<WorkItemResponse>, BadRequest, NotFound, ProblemHttpResult>> GetWorkItemAsync(
        int id, CancellationToken cancellationToken)
    {
        var result = await azureDevOpsService.GetWorkItemAsync(id, cancellationToken);
        return result.ToGetResult<WorkItem, WorkItemResponse>(w => w.Adapt<WorkItemResponse>());
    }

    [HttpPost]
    public async Task<Results<Ok<WorkItemResponse>, BadRequest, NotFound, ProblemHttpResult>> CreateWorkItemAsync(
        [FromBody] CreateWorkItemRequest request, CancellationToken cancellationToken)
    {
        var result = await azureDevOpsService.CreateWorkItemAsync(
            request.Project, request.WorkItemType, request.Title, request.Description,
            request.ParentId, request.AssignedTo, request.AreaPath, request.IterationPath,
            request.Priority, request.Tags, cancellationToken);
        return result.ToPutResult<WorkItem, WorkItemResponse>(w => w.Adapt<WorkItemResponse>());
    }

    [HttpPut("{id:int}")]
    public async Task<Results<Ok<WorkItemResponse>, BadRequest, NotFound, ProblemHttpResult>> UpdateWorkItemAsync(
        int id, [FromBody] UpdateWorkItemRequest request, CancellationToken cancellationToken)
    {
        var result = await azureDevOpsService.UpdateWorkItemAsync(
            id, request.Title, request.Description, request.State, request.AssignedTo,
            request.AreaPath, request.IterationPath, request.Priority, request.Tags, cancellationToken);
        return result.ToPutResult<WorkItem, WorkItemResponse>(w => w.Adapt<WorkItemResponse>());
    }

    [HttpDelete("{id:int}")]
    public async Task<Results<Ok, BadRequest, NotFound, ProblemHttpResult>> DeleteWorkItemAsync(
        int id, CancellationToken cancellationToken)
    {
        var result = await azureDevOpsService.DeleteWorkItemAsync(id, cancellationToken);
        return result.ToOkPostResult();
    }

    [HttpPost("search")]
    public async Task<Results<Ok<List<WorkItemResponse>>, BadRequest, ProblemHttpResult>> QueryWorkItemsAsync(
        [FromBody] QueryWorkItemsRequest request, CancellationToken cancellationToken)
    {
        var result = await azureDevOpsService.QueryWorkItemsAsync(request.Wiql, cancellationToken);
        return result.ToGetResult<WorkItem, WorkItemResponse>(w => w.Adapt<WorkItemResponse>());
    }

    [HttpGet("{id:int}/comments")]
    public async Task<Results<Ok<List<CommentResponse>>, BadRequest, ProblemHttpResult>> GetCommentsAsync(
        int id, [FromQuery] string project, CancellationToken cancellationToken)
    {
        var result = await azureDevOpsService.GetCommentsAsync(project, id, cancellationToken);
        return result.ToGetResult<Comment, CommentResponse>(c => c.Adapt<CommentResponse>());
    }

    [HttpPost("{id:int}/comments")]
    public async Task<Results<Ok<CommentResponse>, BadRequest, NotFound, ProblemHttpResult>> AddCommentAsync(
        int id, [FromQuery] string project, [FromBody] AddCommentRequest request, CancellationToken cancellationToken)
    {
        var result = await azureDevOpsService.AddCommentAsync(project, id, request.Text, cancellationToken);
        return result.ToPutResult<Comment, CommentResponse>(c => c.Adapt<CommentResponse>());
    }
}
