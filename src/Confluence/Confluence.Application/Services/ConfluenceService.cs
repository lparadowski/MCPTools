using Confluence.Application.Interfaces;
using Shared.Application.ResultErrors;
using Confluence.Domain.Entities;
using FluentResults;
using Shared.Application.Chunking;

namespace Confluence.Application.Services;

public interface IConfluenceService
{
    Task<Result<List<Space>>> GetSpacesAsync(CancellationToken cancellationToken = default);
    Task<Result<Space>> GetSpaceByIdAsync(string spaceId, CancellationToken cancellationToken = default);
    Task<Result<Space>> CreateSpaceAsync(string name, string key, string? description, CancellationToken cancellationToken = default);
    Task<Result> DeleteSpaceAsync(string spaceId, CancellationToken cancellationToken = default);
    Task<Result<List<Page>>> GetPagesBySpaceIdAsync(string spaceId, CancellationToken cancellationToken = default);
    Task<Result<Page>> GetPageByIdAsync(string pageId, CancellationToken cancellationToken = default);
    Task<Result<ChunkedResult<Page>>> GetPageByIdAsync(string pageId, int offset, int maxLength, CancellationToken cancellationToken = default);
    Task<Result<Page>> CreatePageAsync(string spaceId, string title, string? body, string? parentId, CancellationToken cancellationToken = default);
    Task<Result<Page>> UpdatePageAsync(string pageId, string title, string? body, int version, CancellationToken cancellationToken = default);
    Task<Result> DeletePageAsync(string pageId, CancellationToken cancellationToken = default);
    Task<Result<List<SearchResult>>> SearchAsync(string cql, int maxResults = 25, CancellationToken cancellationToken = default);
    Task<Result<List<ActivityItem>>> GetUserActivityAsync(string accountId, DateTime startDate, DateTime endDate, CancellationToken cancellationToken = default);
}

public class ConfluenceService(IConfluenceClient confluenceClient, ChunkingSettings chunkingSettings) : IConfluenceService
{
    public async Task<Result<List<Space>>> GetSpacesAsync(CancellationToken cancellationToken = default)
    {
        var spaces = await confluenceClient.GetSpacesAsync(cancellationToken);
        return Result.Ok(spaces);
    }

    public async Task<Result<Space>> GetSpaceByIdAsync(string spaceId, CancellationToken cancellationToken = default)
    {
        var space = await confluenceClient.GetSpaceByIdAsync(spaceId, cancellationToken);

        if (space is null)
        {
            return Result.Fail<Space>(new EntityDoesNotExistError());
        }

        return Result.Ok(space);
    }

    public async Task<Result<Space>> CreateSpaceAsync(string name, string key, string? description, CancellationToken cancellationToken = default)
    {
        var space = await confluenceClient.CreateSpaceAsync(name, key, description, cancellationToken);

        if (space is null)
        {
            return Result.Fail<Space>(new EntityDoesNotExistError());
        }

        return Result.Ok(space);
    }

    public async Task<Result> DeleteSpaceAsync(string spaceId, CancellationToken cancellationToken = default)
    {
        var success = await confluenceClient.DeleteSpaceAsync(spaceId, cancellationToken);

        if (!success)
        {
            return Result.Fail(new EntityDoesNotExistError());
        }

        return Result.Ok();
    }

    public async Task<Result<List<Page>>> GetPagesBySpaceIdAsync(string spaceId, CancellationToken cancellationToken = default)
    {
        var pages = await confluenceClient.GetPagesBySpaceIdAsync(spaceId, cancellationToken);
        return Result.Ok(pages);
    }

    public async Task<Result<Page>> GetPageByIdAsync(string pageId, CancellationToken cancellationToken = default)
    {
        var page = await confluenceClient.GetPageByIdAsync(pageId, cancellationToken);

        if (page is null)
        {
            return Result.Fail<Page>(new EntityDoesNotExistError());
        }

        return Result.Ok(page);
    }

    public async Task<Result<ChunkedResult<Page>>> GetPageByIdAsync(string pageId, int offset, int maxLength, CancellationToken cancellationToken = default)
    {
        var page = await confluenceClient.GetPageByIdAsync(pageId, cancellationToken);

        if (page is null)
        {
            return Result.Fail<ChunkedResult<Page>>(new EntityDoesNotExistError());
        }

        var effectiveMaxLength = maxLength > 0 ? maxLength : chunkingSettings.DefaultMaxLength;
        var result = new ChunkedResult<Page> { Value = page };

        if (!string.IsNullOrEmpty(page.Body) && page.Body.Length > effectiveMaxLength)
        {
            var chunked = ContentChunker.Chunk(page.Body, offset, effectiveMaxLength);
            page.Body = chunked.Content;
            result.ChunkMetadata = chunked;
        }

        return Result.Ok(result);
    }

    public async Task<Result<Page>> CreatePageAsync(string spaceId, string title, string? body, string? parentId, CancellationToken cancellationToken = default)
    {
        var page = await confluenceClient.CreatePageAsync(spaceId, title, body, parentId, cancellationToken);

        if (page is null)
        {
            return Result.Fail<Page>(new EntityDoesNotExistError());
        }

        return Result.Ok(page);
    }

    public async Task<Result<Page>> UpdatePageAsync(string pageId, string title, string? body, int version, CancellationToken cancellationToken = default)
    {
        var page = await confluenceClient.UpdatePageAsync(pageId, title, body, version, cancellationToken);

        if (page is null)
        {
            return Result.Fail<Page>(new EntityDoesNotExistError());
        }

        return Result.Ok(page);
    }

    public async Task<Result> DeletePageAsync(string pageId, CancellationToken cancellationToken = default)
    {
        var success = await confluenceClient.DeletePageAsync(pageId, cancellationToken);

        if (!success)
        {
            return Result.Fail(new EntityDoesNotExistError());
        }

        return Result.Ok();
    }

    public async Task<Result<List<SearchResult>>> SearchAsync(string cql, int maxResults = 25, CancellationToken cancellationToken = default)
    {
        var results = await confluenceClient.SearchAsync(cql, maxResults, cancellationToken);
        return Result.Ok(results);
    }

    public async Task<Result<List<ActivityItem>>> GetUserActivityAsync(string accountId, DateTime startDate, DateTime endDate, CancellationToken cancellationToken = default)
    {
        var items = await confluenceClient.GetUserActivityAsync(accountId, startDate, endDate, cancellationToken);
        return Result.Ok(items);
    }
}
