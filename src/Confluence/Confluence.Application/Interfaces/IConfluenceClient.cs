using Confluence.Domain.Entities;

namespace Confluence.Application.Interfaces;

public interface IConfluenceClient
{
    Task<List<Space>> GetSpacesAsync(CancellationToken cancellationToken = default);
    Task<Space?> GetSpaceByIdAsync(string spaceId, CancellationToken cancellationToken = default);
    Task<Space?> CreateSpaceAsync(string name, string key, string? description, CancellationToken cancellationToken = default);
    Task<bool> DeleteSpaceAsync(string spaceId, CancellationToken cancellationToken = default);
    Task<List<Page>> GetPagesBySpaceIdAsync(string spaceId, CancellationToken cancellationToken = default);
    Task<Page?> GetPageByIdAsync(string pageId, CancellationToken cancellationToken = default);
    Task<Page?> CreatePageAsync(string spaceId, string title, string? body, string? parentId, CancellationToken cancellationToken = default);
    Task<Page?> UpdatePageAsync(string pageId, string title, string? body, int version, CancellationToken cancellationToken = default);
    Task<bool> DeletePageAsync(string pageId, CancellationToken cancellationToken = default);
    Task<List<SearchResult>> SearchAsync(string cql, int maxResults = 25, CancellationToken cancellationToken = default);
}
