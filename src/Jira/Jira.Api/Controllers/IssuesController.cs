using Asp.Versioning;
using Shared.Api.Extensions;
using Jira.Api.Requests;
using Jira.Api.Responses;
using Jira.Application.Interfaces;
using Jira.Domain.Entities;
using Mapster;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;
using Shared.Application.Chunking;

namespace Jira.Api.Controllers;

[ApiController]
[ApiVersion(Versions.V1)]
[Route("api/v{version:apiVersion}/[controller]")]
public class IssuesController(IJiraService jiraService) : ControllerBase
{
    [HttpPost]
    public async Task<Results<Ok<IssueResponse>, BadRequest, NotFound, ProblemHttpResult>> CreateIssueAsync(
        [FromBody] CreateIssueRequest request, CancellationToken cancellationToken)
    {
        var result = await jiraService.CreateIssueAsync(
            request.ProjectKey,
            request.IssueType,
            request.Summary,
            request.Description,
            request.CustomFields,
            request.ParentKey,
            request.Labels,
            cancellationToken);
        return result.ToPutResult<Issue, IssueResponse>(i => i.Adapt<IssueResponse>());
    }

    [HttpGet("{issueKeyOrId}")]
    public async Task<Results<Ok<IssueResponse>, BadRequest, NotFound, ProblemHttpResult>> GetIssueAsync(
        string issueKeyOrId, [FromQuery] int offset = 0, [FromQuery] int maxLength = 0,
        CancellationToken cancellationToken = default)
    {
        var result = await jiraService.GetIssueAsync(issueKeyOrId, offset, maxLength, cancellationToken);
        return result.ToGetResult<ChunkedResult<Issue>, IssueResponse>(chunkedResult =>
        {
            var response = chunkedResult.Value.Adapt<IssueResponse>();

            if (chunkedResult.ChunkMetadata is not null)
            {
                response.TotalDescriptionLength = chunkedResult.ChunkMetadata.TotalLength;
                response.HasMore = chunkedResult.ChunkMetadata.HasMore;
                response.NextOffset = chunkedResult.ChunkMetadata.NextOffset;
            }

            return response;
        });
    }

    [HttpPut("{issueKeyOrId}")]
    public async Task<Results<Ok<IssueResponse>, BadRequest, NotFound, ProblemHttpResult>> UpdateIssueAsync(
        string issueKeyOrId, [FromBody] UpdateIssueRequest request, CancellationToken cancellationToken)
    {
        var result = await jiraService.UpdateIssueAsync(issueKeyOrId, request.Summary, request.Description, request.CustomFields, cancellationToken);
        return result.ToPutResult<Issue, IssueResponse>(i => i.Adapt<IssueResponse>());
    }

    [HttpDelete("{issueKeyOrId}")]
    public async Task<Results<Ok, BadRequest, NotFound, ProblemHttpResult>> DeleteIssueAsync(
        string issueKeyOrId, CancellationToken cancellationToken)
    {
        var result = await jiraService.DeleteIssueAsync(issueKeyOrId, cancellationToken);
        return result.ToOkPostResult();
    }

    [HttpPost("search")]
    public async Task<Results<Ok<ChunkedContentResponse>, BadRequest, NotFound, ProblemHttpResult>> SearchIssuesAsync(
        [FromBody] SearchIssuesRequest request, [FromQuery] int offset = 0, [FromQuery] int maxLength = 0,
        CancellationToken cancellationToken = default)
    {
        var result = await jiraService.SearchIssuesAsync(request.Jql, offset, maxLength, request.MaxResults, cancellationToken);
        return result.ToGetResult<ChunkedResult<List<Issue>>, ChunkedContentResponse>(chunkedResult =>
        {
            var issues = chunkedResult.Value.Select(i => i.Adapt<IssueResponse>()).ToList();
            var serialized = System.Text.Json.JsonSerializer.Serialize(issues);

            if (chunkedResult.ChunkMetadata is not null)
            {
                return new ChunkedContentResponse
                {
                    Content = chunkedResult.ChunkMetadata.Content,
                    TotalLength = chunkedResult.ChunkMetadata.TotalLength,
                    HasMore = chunkedResult.ChunkMetadata.HasMore,
                    NextOffset = chunkedResult.ChunkMetadata.NextOffset
                };
            }

            return new ChunkedContentResponse
            {
                Content = serialized,
                TotalLength = serialized.Length,
                HasMore = false,
                NextOffset = null
            };
        });
    }

    [HttpGet("{issueKeyOrId}/transitions")]
    public async Task<Results<Ok<List<TransitionResponse>>, BadRequest, ProblemHttpResult>> GetTransitionsAsync(
        string issueKeyOrId, CancellationToken cancellationToken)
    {
        var result = await jiraService.GetTransitionsAsync(issueKeyOrId, cancellationToken);
        return result.ToGetResult<Transition, TransitionResponse>(t => t.Adapt<TransitionResponse>());
    }

    [HttpPost("{issueKeyOrId}/transitions")]
    public async Task<Results<Ok, BadRequest, NotFound, ProblemHttpResult>> TransitionIssueAsync(
        string issueKeyOrId, [FromBody] TransitionIssueRequest request, CancellationToken cancellationToken)
    {
        var result = await jiraService.TransitionIssueAsync(issueKeyOrId, request.TransitionId, cancellationToken);
        return result.ToOkPostResult();
    }

    [HttpGet("{issueKeyOrId}/comments")]
    public async Task<Results<Ok<List<CommentResponse>>, BadRequest, ProblemHttpResult>> GetCommentsAsync(
        string issueKeyOrId, CancellationToken cancellationToken)
    {
        var result = await jiraService.GetCommentsAsync(issueKeyOrId, cancellationToken);
        return result.ToGetResult<Comment, CommentResponse>(c => c.Adapt<CommentResponse>());
    }

    [HttpPost("{issueKeyOrId}/comments")]
    public async Task<Results<Ok<CommentResponse>, BadRequest, NotFound, ProblemHttpResult>> AddCommentAsync(
        string issueKeyOrId, [FromBody] AddCommentRequest request, CancellationToken cancellationToken)
    {
        var result = await jiraService.AddCommentAsync(issueKeyOrId, request.Body, cancellationToken);
        return result.ToPutResult<Comment, CommentResponse>(c => c.Adapt<CommentResponse>());
    }

    [HttpPost("{issueKeyOrId}/labels")]
    public async Task<Results<Ok, BadRequest, NotFound, ProblemHttpResult>> AddLabelAsync(
        string issueKeyOrId, [FromBody] LabelRequest request, CancellationToken cancellationToken)
    {
        var result = await jiraService.AddLabelAsync(issueKeyOrId, request.Label, cancellationToken);
        return result.ToOkPostResult();
    }

    [HttpDelete("{issueKeyOrId}/labels/{label}")]
    public async Task<Results<Ok, BadRequest, NotFound, ProblemHttpResult>> RemoveLabelAsync(
        string issueKeyOrId, string label, CancellationToken cancellationToken)
    {
        var result = await jiraService.RemoveLabelAsync(issueKeyOrId, label, cancellationToken);
        return result.ToOkPostResult();
    }

    [HttpPut("{issueKeyOrId}/assignee")]
    public async Task<Results<Ok, BadRequest, NotFound, ProblemHttpResult>> AssignIssueAsync(
        string issueKeyOrId, [FromBody] AssignIssueRequest request, CancellationToken cancellationToken)
    {
        var result = await jiraService.AssignIssueAsync(issueKeyOrId, request.AccountId, cancellationToken);
        return result.ToOkPostResult();
    }

    [HttpGet("{issueKeyOrId}/worklogs")]
    public async Task<Results<Ok<List<WorklogResponse>>, BadRequest, ProblemHttpResult>> GetWorklogsAsync(
        string issueKeyOrId, CancellationToken cancellationToken)
    {
        var result = await jiraService.GetWorklogsAsync(issueKeyOrId, cancellationToken);
        return result.ToGetResult<Worklog, WorklogResponse>(w => w.Adapt<WorklogResponse>());
    }

    [HttpGet("{issueKeyOrId}/profile")]
    public async Task<Results<Ok<TicketProfileResponse>, BadRequest, NotFound, ProblemHttpResult>> GetTicketProfileAsync(
        string issueKeyOrId, CancellationToken cancellationToken)
    {
        var result = await jiraService.GetTicketProfileAsync(issueKeyOrId, cancellationToken);
        return result.ToGetResult<TicketProfile, TicketProfileResponse>(p => p.Adapt<TicketProfileResponse>());
    }

    [HttpPost("{issueKeyOrId}/worklogs")]
    public async Task<Results<Ok<WorklogResponse>, BadRequest, NotFound, ProblemHttpResult>> AddWorklogAsync(
        string issueKeyOrId, [FromBody] AddWorklogRequest request, CancellationToken cancellationToken)
    {
        var result = await jiraService.AddWorklogAsync(issueKeyOrId, request.TimeSpent, request.Comment, request.Started?.DateTime, cancellationToken);
        return result.ToPutResult<Worklog, WorklogResponse>(w => w.Adapt<WorklogResponse>());
    }
}
