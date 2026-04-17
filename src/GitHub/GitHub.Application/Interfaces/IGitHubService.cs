using FluentResults;
using GitHub.Domain.Entities;

namespace GitHub.Application.Interfaces;

public interface IGitHubService
{
    Task<Result<List<Repository>>> GetRepositoriesAsync(CancellationToken cancellationToken = default);
    Task<Result<Repository>> GetRepositoryAsync(string owner, string repo, CancellationToken cancellationToken = default);
    Task<Result<List<ActivityEvent>>> GetUserActivityAsync(string? username = null, DateOnly? from = null, DateOnly? to = null, CancellationToken cancellationToken = default);
}
