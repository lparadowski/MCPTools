using GitHub.Domain.Entities;

namespace GitHub.Application.Interfaces;

public interface IGitHubClient
{
    Task<List<Repository>> GetRepositoriesAsync(CancellationToken cancellationToken = default);
    Task<Repository?> GetRepositoryAsync(string owner, string repo, CancellationToken cancellationToken = default);
    Task<List<ActivityEvent>> GetUserActivityAsync(string? username = null, DateOnly? from = null, DateOnly? to = null, CancellationToken cancellationToken = default);
}
