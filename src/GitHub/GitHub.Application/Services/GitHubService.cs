using FluentResults;
using GitHub.Application.Interfaces;
using GitHub.Domain.Entities;
using Shared.Application.ResultErrors;

namespace GitHub.Application.Services;

public class GitHubService(IGitHubClient gitHubClient) : IGitHubService
{
    public async Task<Result<List<Repository>>> GetRepositoriesAsync(CancellationToken cancellationToken = default)
    {
        var repositories = await gitHubClient.GetRepositoriesAsync(cancellationToken);
        return Result.Ok(repositories);
    }

    public async Task<Result<Repository>> GetRepositoryAsync(string owner, string repo, CancellationToken cancellationToken = default)
    {
        var repository = await gitHubClient.GetRepositoryAsync(owner, repo, cancellationToken);

        if (repository is null)
        {
            return Result.Fail<Repository>(new EntityDoesNotExistError());
        }

        return Result.Ok(repository);
    }

    public async Task<Result<List<ActivityEvent>>> GetUserActivityAsync(string? username = null, DateOnly? from = null, DateOnly? to = null, CancellationToken cancellationToken = default)
    {
        var events = await gitHubClient.GetUserActivityAsync(username, from, to, cancellationToken);
        return Result.Ok(events);
    }

    // Pull Requests

    public async Task<Result<List<PullRequest>>> GetPullRequestsAsync(string owner, string repo, string? state = null, int maxResults = 30, CancellationToken cancellationToken = default)
    {
        var pullRequests = await gitHubClient.GetPullRequestsAsync(owner, repo, state, maxResults, cancellationToken);
        return Result.Ok(pullRequests);
    }

    public async Task<Result<PullRequest>> GetPullRequestAsync(string owner, string repo, int number, CancellationToken cancellationToken = default)
    {
        var pullRequest = await gitHubClient.GetPullRequestAsync(owner, repo, number, cancellationToken);

        if (pullRequest is null)
        {
            return Result.Fail<PullRequest>(new EntityDoesNotExistError());
        }

        return Result.Ok(pullRequest);
    }
}
