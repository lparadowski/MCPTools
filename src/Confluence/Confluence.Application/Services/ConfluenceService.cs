using Confluence.Application.Interfaces;
using Confluence.Application.ResultErrors;
using Confluence.Domain.Entities;
using FluentResults;

namespace Confluence.Application.Services;

public interface IConfluenceService
{
    Task<Result<List<Space>>> GetSpacesAsync(CancellationToken cancellationToken = default);
    Task<Result<Space>> GetSpaceByIdAsync(string spaceId, CancellationToken cancellationToken = default);
    Task<Result<Space>> CreateSpaceAsync(string name, string key, string? description, CancellationToken cancellationToken = default);
    Task<Result> DeleteSpaceAsync(string spaceId, CancellationToken cancellationToken = default);
    Task<Result<List<Page>>> GetPagesBySpaceIdAsync(string spaceId, CancellationToken cancellationToken = default);
    Task<Result<Page>> GetPageByIdAsync(string pageId, CancellationToken cancellationToken = default);
    Task<Result<Page>> CreatePageAsync(string spaceId, string title, string? body, string? parentId, CancellationToken cancellationToken = default);
    Task<Result<Page>> UpdatePageAsync(string pageId, string title, string? body, int version, CancellationToken cancellationToken = default);
    Task<Result> DeletePageAsync(string pageId, CancellationToken cancellationToken = default);
}

public class ConfluenceService(IConfluenceClient confluenceClient) : IConfluenceService
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
}
