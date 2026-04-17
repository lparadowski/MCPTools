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

        var allEvents = new List<GitHubEventDto>();
        for (var page = 1; page <= 10; page++)
        {
            var response = await http.GetAsync($"/users/{username}/events?per_page=100&page={page}", cancellationToken);

            if (!response.IsSuccessStatusCode)
                break;

            var events = await response.Content.ReadFromJsonAsync<List<GitHubEventDto>>(cancellationToken: cancellationToken);

            if (events is null || events.Count == 0)
                break;

            allEvents.AddRange(events);

            var oldest = DateOnly.FromDateTime(events.Last().CreatedAt);
            if (oldest < fromDate)
                break;
        }

        return allEvents
            .Where(e =>
            {
                var eventDate = DateOnly.FromDateTime(e.CreatedAt);
                return eventDate >= fromDate && eventDate <= toDate;
            })
            .Select(MapActivityEvent)
            .ToList();
    }

    private static ActivityEvent MapActivityEvent(GitHubEventDto dto) => new()
    {
        Id = dto.Id,
        Type = dto.Type,
        Repo = dto.Repo?.Name ?? string.Empty,
        Action = dto.Payload?.Action,
        Ref = dto.Payload?.Ref?.Replace("refs/heads/", "") ?? dto.Payload?.PullRequest?.Head?.Ref,
        Number = dto.Payload?.PullRequest?.Number ?? dto.Payload?.Issue?.Number ?? dto.Payload?.Number,
        Title = dto.Payload?.PullRequest?.Title ?? dto.Payload?.Issue?.Title,
        Body = dto.Payload?.Comment?.Body ?? dto.Payload?.Review?.Body,
        CommitMessages = dto.Payload?.Commits?
            .Select(c => c.Message?.Split('\n').FirstOrDefault() ?? string.Empty)
            .Where(m => !string.IsNullOrEmpty(m))
            .ToList() ?? [],
        CommitCount = dto.Payload?.Size ?? dto.Payload?.Commits?.Count ?? 0,
        CreatedAt = dto.CreatedAt
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
