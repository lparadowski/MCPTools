using Asp.Versioning;
using Shared.Api.Extensions;
using Jira.Api.Requests;
using Jira.Application.Interfaces;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;

namespace Jira.Api.Controllers;

[ApiController]
[ApiVersion(Versions.V1)]
[Route("api/v{version:apiVersion}/[controller]")]
public class IssueLinksController(IJiraService jiraService) : ControllerBase
{
    [HttpPost]
    public async Task<Results<Ok, BadRequest, NotFound, ProblemHttpResult>> LinkIssuesAsync(
        [FromBody] LinkIssuesRequest request, CancellationToken cancellationToken)
    {
        var result = await jiraService.LinkIssuesAsync(request.InwardIssueKey, request.OutwardIssueKey, request.LinkTypeName, cancellationToken);
        return result.ToOkPostResult();
    }
}
