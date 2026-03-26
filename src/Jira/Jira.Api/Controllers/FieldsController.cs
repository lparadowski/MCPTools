using Asp.Versioning;
using Jira.Application.Interfaces;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;

namespace Jira.Api.Controllers;

[ApiController]
[ApiVersion(Versions.V1)]
[Route("api/v{version:apiVersion}/[controller]")]
public class FieldsController(IJiraService jiraService) : ControllerBase
{
    [HttpGet]
    public async Task<Results<Ok<Dictionary<string, string>>, ProblemHttpResult>> GetFieldsAsync(
        CancellationToken cancellationToken)
    {
        var result = await jiraService.GetFieldsAsync(cancellationToken);

        if (result.IsSuccess)
        {
            return TypedResults.Ok(result.Value);
        }

        return TypedResults.Problem(statusCode: StatusCodes.Status500InternalServerError);
    }
}
