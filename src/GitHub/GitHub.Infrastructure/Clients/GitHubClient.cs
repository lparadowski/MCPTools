using System.Net.Http.Json;
using System.Text.Json;
using GitHub.Application.Interfaces;
using GitHub.Domain.Entities;
using GitHub.Infrastructure.Dtos;

namespace GitHub.Infrastructure.Clients;

public class GitHubClient(IHttpClientFactory httpClientFactory) : IGitHubClient
{
    public async Task<List<Repository>> GetRepositoriesAsync(CancellationToken cancellationToken = default)
    {
        var http = httpClientFactory.CreateClient("GitHubApi");
        var response = await http.GetAsync("/user/repos?per_page=100&sort=updated", cancellationToken);
        response.EnsureSuccessStatusCode();

        var dtos = await response.Content.ReadFromJsonAsync<List<GitHubRepositoryDto>>(cancellationToken: cancellationToken);
        return dtos?.Select(MapRepository).ToList() ?? [];
    }

    public async Task<Repository?> GetRepositoryAsync(string owner, string repo, CancellationToken cancellationToken = default)
    {
        var http = httpClientFactory.CreateClient("GitHubApi");
        var response = await http.GetAsync($"/repos/{owner}/{repo}", cancellationToken);

        if (!response.IsSuccessStatusCode)
        {
            return null;
        }

        var dto = await response.Content.ReadFromJsonAsync<GitHubRepositoryDto>(cancellationToken: cancellationToken);
        return dto is not null ? MapRepository(dto) : null;
    }

    public async Task<List<ActivityEvent>> GetUserActivityAsync(string? username = null, DateOnly? from = null, DateOnly? to = null, CancellationToken cancellationToken = default)
    {
        var http = httpClientFactory.CreateClient("GitHubApi");

        if (string.IsNullOrWhiteSpace(username))
        {
            var userResponse = await http.GetAsync("/user", cancellationToken);
            userResponse.EnsureSuccessStatusCode();
            var userDoc = await userResponse.Content.ReadFromJsonAsync<JsonElement>(cancellationToken: cancellationToken);
            username = userDoc.GetProperty("login").GetString()!;
        }

        var fromDate = from ?? DateOnly.FromDateTime(DateTime.UtcNow.Date);
        var toDate = to ?? DateOnly.FromDateTime(DateTime.UtcNow.Date);
        var dateRange = $"{fromDate:yyyy-MM-dd}..{toDate:yyyy-MM-dd}";

        var prsCreatedTask = SearchIssuesAsync(http, $"author:{username} type:pr created:{dateRange}", cancellationToken);
        var prsReviewedTask = SearchIssuesAsync(http, $"reviewed-by:{username} type:pr -author:{username} updated:{dateRange}", cancellationToken);
        var issuesCreatedTask = SearchIssuesAsync(http, $"author:{username} type:issue created:{dateRange}", cancellationToken);
        var commentedOnTask = SearchIssuesAsync(http, $"commenter:{username} -author:{username} updated:{dateRange}", cancellationToken);
        var commitsTask = SearchCommitsAsync(http, $"author:{username} author-date:{dateRange}", cancellationToken);

        await Task.WhenAll(prsCreatedTask, prsReviewedTask, issuesCreatedTask, commentedOnTask, commitsTask);

        var events = new List<ActivityEvent>();

        foreach (var pr in prsCreatedTask.Result)
        {
            events.Add(new ActivityEvent
            {
                Id = pr.Id.ToString(),
                Type = "PullRequest",
                Action = "created",
                Repo = ExtractRepoName(pr.RepositoryUrl),
                Number = pr.Number,
                Title = pr.Title,
                Body = pr.Body,
                CreatedAt = pr.CreatedAt
            });
        }

        foreach (var pr in prsReviewedTask.Result)
        {
            events.Add(new ActivityEvent
            {
                Id = $"review-{pr.Id}",
                Type = "PullRequestReview",
                Action = "reviewed",
                Repo = ExtractRepoName(pr.RepositoryUrl),
                Number = pr.Number,
                Title = pr.Title,
                CreatedAt = pr.UpdatedAt ?? pr.CreatedAt
            });
        }

        foreach (var issue in issuesCreatedTask.Result)
        {
            events.Add(new ActivityEvent
            {
                Id = issue.Id.ToString(),
                Type = "Issue",
                Action = "created",
                Repo = ExtractRepoName(issue.RepositoryUrl),
                Number = issue.Number,
                Title = issue.Title,
                Body = issue.Body,
                CreatedAt = issue.CreatedAt
            });
        }

        foreach (var item in commentedOnTask.Result)
        {
            var isPr = item.PullRequest is not null;
            events.Add(new ActivityEvent
            {
                Id = $"comment-{item.Id}",
                Type = isPr ? "PullRequestComment" : "IssueComment",
                Action = "commented",
                Repo = ExtractRepoName(item.RepositoryUrl),
                Number = item.Number,
                Title = item.Title,
                CreatedAt = item.UpdatedAt ?? item.CreatedAt
            });
        }

        foreach (var commit in commitsTask.Result)
        {
            events.Add(new ActivityEvent
            {
                Id = commit.Sha,
                Type = "Commit",
                Repo = commit.Repository?.FullName ?? string.Empty,
                CommitMessages = [commit.Commit?.Message?.Split('\n').FirstOrDefault() ?? string.Empty],
                CommitCount = 1,
                CreatedAt = commit.Commit?.Author?.Date ?? DateTime.MinValue
            });
        }

        // Enrich comment/review events with actual body text
        var enrichTasks = events
            .Where(e => e.Type is "PullRequestComment" or "IssueComment" or "PullRequestReview")
            .Select(e => EnrichActivityEventAsync(http, e, username, fromDate, toDate, cancellationToken))
            .ToList();

        if (enrichTasks.Count > 0)
            await Task.WhenAll(enrichTasks);

        return events.OrderByDescending(e => e.CreatedAt).ToList();
    }

    private async Task EnrichActivityEventAsync(HttpClient http, ActivityEvent activityEvent, string username, DateOnly fromDate, DateOnly toDate, CancellationToken cancellationToken)
    {
        var repoParts = activityEvent.Repo.Split('/');
        if (repoParts.Length != 2 || activityEvent.Number is null)
            return;

        var owner = repoParts[0];
        var repo = repoParts[1];
        var number = activityEvent.Number.Value;

        if (activityEvent.Type == "PullRequestReview")
        {
            var reviews = await GetPullRequestReviewsAsync(owner, repo, number, cancellationToken);
            var userReviews = reviews
                .Where(r => r.Author.Equals(username, StringComparison.OrdinalIgnoreCase))
                .Where(r => DateOnly.FromDateTime(r.SubmittedAt) >= fromDate && DateOnly.FromDateTime(r.SubmittedAt) <= toDate)
                .ToList();

            activityEvent.Body = string.Join("\n---\n", userReviews.Select(r =>
                $"[{r.State}] {(string.IsNullOrWhiteSpace(r.Body) ? "(no comment)" : r.Body)}"));
        }
        else
        {
            var comments = await GetIssueCommentsAsync(owner, repo, number, cancellationToken);
            var userComments = comments
                .Where(c => c.Author.Equals(username, StringComparison.OrdinalIgnoreCase))
                .Where(c => DateOnly.FromDateTime(c.CreatedAt) >= fromDate && DateOnly.FromDateTime(c.CreatedAt) <= toDate)
                .ToList();

            activityEvent.Body = string.Join("\n---\n", userComments.Select(c => c.Body));
        }
    }

    private static async Task<List<GitHubSearchIssueItemDto>> SearchIssuesAsync(HttpClient http, string query, CancellationToken cancellationToken)
    {
        var response = await http.GetAsync($"/search/issues?q={Uri.EscapeDataString(query)}&per_page=100", cancellationToken);

        if (!response.IsSuccessStatusCode)
            return [];

        var result = await response.Content.ReadFromJsonAsync<GitHubSearchResultDto<GitHubSearchIssueItemDto>>(cancellationToken: cancellationToken);
        return result?.Items ?? [];
    }

    private static async Task<List<GitHubSearchCommitItemDto>> SearchCommitsAsync(HttpClient http, string query, CancellationToken cancellationToken)
    {
        var response = await http.GetAsync($"/search/commits?q={Uri.EscapeDataString(query)}&per_page=100", cancellationToken);

        if (!response.IsSuccessStatusCode)
            return [];

        var result = await response.Content.ReadFromJsonAsync<GitHubSearchResultDto<GitHubSearchCommitItemDto>>(cancellationToken: cancellationToken);
        return result?.Items ?? [];
    }

    private static string ExtractRepoName(string? repositoryUrl)
    {
        if (string.IsNullOrEmpty(repositoryUrl))
            return string.Empty;

        // repository_url format: https://api.github.com/repos/owner/repo
        var parts = repositoryUrl.Split('/');
        return parts.Length >= 2 ? $"{parts[^2]}/{parts[^1]}" : repositoryUrl;
    }

    // Pull Requests

    public async Task<List<PullRequest>> GetPullRequestsAsync(string owner, string repo, string? state = null, int maxResults = 30, CancellationToken cancellationToken = default)
    {
        var http = httpClientFactory.CreateClient("GitHubApi");
        var url = $"/repos/{owner}/{repo}/pulls?per_page={maxResults}&state={state ?? "open"}";

        var response = await http.GetAsync(url, cancellationToken);
        response.EnsureSuccessStatusCode();

        var dtos = await response.Content.ReadFromJsonAsync<List<GitHubPullRequestDto>>(cancellationToken: cancellationToken);
        return dtos?.Select(MapPullRequest).ToList() ?? [];
    }

    public async Task<PullRequest?> GetPullRequestAsync(string owner, string repo, int number, CancellationToken cancellationToken = default)
    {
        var http = httpClientFactory.CreateClient("GitHubApi");
        var response = await http.GetAsync($"/repos/{owner}/{repo}/pulls/{number}", cancellationToken);

        if (!response.IsSuccessStatusCode)
        {
            return null;
        }

        var dto = await response.Content.ReadFromJsonAsync<GitHubPullRequestDto>(cancellationToken: cancellationToken);
        return dto is not null ? MapPullRequest(dto) : null;
    }

    private static PullRequest MapPullRequest(GitHubPullRequestDto dto) => new()
    {
        Number = dto.Number,
        Title = dto.Title,
        State = dto.MergedAt is not null ? "merged" : dto.State,
        Author = dto.User?.Login,
        Body = dto.Body,
        HeadBranch = dto.Head?.Ref,
        BaseBranch = dto.Base?.Ref,
        Draft = dto.Draft,
        Url = dto.HtmlUrl,
        CreatedAt = dto.CreatedAt,
        UpdatedAt = dto.UpdatedAt,
        MergedAt = dto.MergedAt,
        Additions = dto.Additions,
        Deletions = dto.Deletions,
        ChangedFiles = dto.ChangedFiles,
        ReviewComments = dto.ReviewComments
    };

    // Search

    public async Task<List<PullRequest>> SearchPullRequestsAsync(string query, CancellationToken cancellationToken = default)
    {
        var http = httpClientFactory.CreateClient("GitHubApi");
        var searchResults = await SearchIssuesAsync(http, $"{query} type:pr", cancellationToken);

        var pullRequests = new List<PullRequest>();
        foreach (var result in searchResults)
        {
            var repo = ExtractRepoName(result.RepositoryUrl);
            var parts = repo.Split('/');
            if (parts.Length != 2) continue;

            var pr = await GetPullRequestAsync(parts[0], parts[1], result.Number, cancellationToken);
            if (pr is not null)
                pullRequests.Add(pr);
        }

        return pullRequests;
    }

    // Comments & Reviews

    public async Task<List<IssueComment>> GetIssueCommentsAsync(string owner, string repo, int number, CancellationToken cancellationToken = default)
    {
        var http = httpClientFactory.CreateClient("GitHubApi");
        var response = await http.GetAsync($"/repos/{owner}/{repo}/issues/{number}/comments?per_page=100", cancellationToken);

        if (!response.IsSuccessStatusCode)
            return [];

        var dtos = await response.Content.ReadFromJsonAsync<List<GitHubIssueCommentDto>>(cancellationToken: cancellationToken);
        return dtos?.Select(MapIssueComment).ToList() ?? [];
    }

    public async Task<List<Review>> GetPullRequestReviewsAsync(string owner, string repo, int number, CancellationToken cancellationToken = default)
    {
        var http = httpClientFactory.CreateClient("GitHubApi");
        var response = await http.GetAsync($"/repos/{owner}/{repo}/pulls/{number}/reviews?per_page=100", cancellationToken);

        if (!response.IsSuccessStatusCode)
            return [];

        var dtos = await response.Content.ReadFromJsonAsync<List<GitHubReviewDto>>(cancellationToken: cancellationToken);
        return dtos?.Select(MapReview).ToList() ?? [];
    }

    private static IssueComment MapIssueComment(GitHubIssueCommentDto dto) => new()
    {
        Id = dto.Id,
        Author = dto.User?.Login ?? string.Empty,
        Body = dto.Body,
        CreatedAt = dto.CreatedAt,
        UpdatedAt = dto.UpdatedAt
    };

    private static Review MapReview(GitHubReviewDto dto) => new()
    {
        Id = dto.Id,
        Author = dto.User?.Login ?? string.Empty,
        State = dto.State,
        Body = dto.Body,
        SubmittedAt = dto.SubmittedAt
    };

    private static Repository MapRepository(GitHubRepositoryDto dto) => new()
    {
        Id = dto.Id.ToString(),
        Name = dto.Name,
        FullName = dto.FullName,
        Description = dto.Description,
        Url = dto.HtmlUrl,
        Private = dto.Private,
        DefaultBranch = dto.DefaultBranch,
        Language = dto.Language,
        CreatedAt = dto.CreatedAt,
        UpdatedAt = dto.UpdatedAt
    };
}
