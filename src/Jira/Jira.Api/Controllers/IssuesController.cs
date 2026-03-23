using Asp.Versioning;
using Jira.Api.Extensions;
using Jira.Api.Requests;
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
        string issueKeyOrId, CancellationToken cancellationToken)
    {
        var result = await jiraService.GetIssueAsync(issueKeyOrId, cancellationToken);
        return result.ToGetResult<Issue, IssueResponse>(i => i.Adapt<IssueResponse>());
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
    public async Task<Results<Ok<List<IssueResponse>>, BadRequest, ProblemHttpResult>> SearchIssuesAsync(
        [FromBody] SearchIssuesRequest request, CancellationToken cancellationToken)
    {
        var result = await jiraService.SearchIssuesAsync(request.Jql, request.MaxResults, cancellationToken);
        return result.ToGetResult<Issue, IssueResponse>(i => i.Adapt<IssueResponse>());
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

    [HttpPost("{issueKeyOrId}/worklogs")]
    public async Task<Results<Ok<WorklogResponse>, BadRequest, NotFound, ProblemHttpResult>> AddWorklogAsync(
        string issueKeyOrId, [FromBody] AddWorklogRequest request, CancellationToken cancellationToken)
    {
        var result = await jiraService.AddWorklogAsync(issueKeyOrId, request.TimeSpent, request.Comment, request.Started, cancellationToken);
        return result.ToPutResult<Worklog, WorklogResponse>(w => w.Adapt<WorklogResponse>());
    }
}
