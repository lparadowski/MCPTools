using FluentResults;
using GitHub.Domain.Entities;

namespace GitHub.Application.Interfaces;

public interface IGitHubService
{
    Task<Result<List<Repository>>> GetRepositoriesAsync(CancellationToken cancellationToken = default);
    Task<Result<Repository>> GetRepositoryAsync(string owner, string repo, CancellationToken cancellationToken = default);
    Task<Result<List<ActivityEvent>>> GetUserActivityAsync(string? username = null, DateOnly? from = null, DateOnly? to = null, CancellationToken cancellationToken = default);

    // Pull Requests
    Task<Result<List<PullRequest>>> GetPullRequestsAsync(string owner, string repo, string? state = null, int maxResults = 30, CancellationToken cancellationToken = default);
    Task<Result<PullRequest>> GetPullRequestAsync(string owner, string repo, int number, CancellationToken cancellationToken = default);

    // Comments & Reviews
    Task<Result<List<IssueComment>>> GetIssueCommentsAsync(string owner, string repo, int number, CancellationToken cancellationToken = default);
    Task<Result<List<Review>>> GetPullRequestReviewsAsync(string owner, string repo, int number, CancellationToken cancellationToken = default);
}
