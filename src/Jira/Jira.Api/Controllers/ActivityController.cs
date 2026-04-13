using Asp.Versioning;
using Shared.Api.Extensions;
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
public class ActivityController(IJiraService jiraService) : ControllerBase
{
    [HttpGet]
    public async Task<Results<Ok<ChunkedContentResponse>, BadRequest, NotFound, ProblemHttpResult>> GetUserActivityAsync(
        [FromQuery] string accountId,
        [FromQuery] DateTime startDate,
        [FromQuery] DateTime endDate,
        [FromQuery] int offset = 0,
        [FromQuery] int maxLength = 0,
        CancellationToken cancellationToken = default)
    {
        var result = await jiraService.GetUserActivityAsync(accountId, startDate, endDate, offset, maxLength, cancellationToken);
        return result.ToGetResult<ChunkedResult<List<UserActivity>>, ChunkedContentResponse>(chunkedResult =>
        {
            var activities = chunkedResult.Value.Select(a => a.Adapt<UserActivityResponse>()).ToList();
            var serialized = System.Text.Json.JsonSerializer.Serialize(activities);

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
}
