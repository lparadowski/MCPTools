using GitHub.Domain.Entities;

namespace GitHub.Application.Interfaces;

public interface IGitHubClient
{
    Task<List<Repository>> GetRepositoriesAsync(CancellationToken cancellationToken = default);
    Task<Repository?> GetRepositoryAsync(string owner, string repo, CancellationToken cancellationToken = default);
    Task<List<ActivityEvent>> GetUserActivityAsync(string? username = null, DateOnly? from = null, DateOnly? to = null, CancellationToken cancellationToken = default);

    // Pull Requests
    Task<List<PullRequest>> GetPullRequestsAsync(string owner, string repo, string? state = null, int maxResults = 30, CancellationToken cancellationToken = default);
    Task<PullRequest?> GetPullRequestAsync(string owner, string repo, int number, CancellationToken cancellationToken = default);

    // Comments & Reviews
    Task<List<IssueComment>> GetIssueCommentsAsync(string owner, string repo, int number, CancellationToken cancellationToken = default);
    Task<List<Review>> GetPullRequestReviewsAsync(string owner, string repo, int number, CancellationToken cancellationToken = default);
}
