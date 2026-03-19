using System.Net.Http.Json;
using System.Text.Json;
using Jira.Application.Interfaces;
using Jira.Domain.Entities;
using Jira.Infrastructure.Dtos;
using Jira.Infrastructure.Parsing;
using Mapster;

namespace Jira.Infrastructure.Clients;

public class JiraClient(IHttpClientFactory httpClientFactory) : IJiraClient
{
    private static readonly HashSet<string> StandardIssueFields =
    [
        "summary",
        "description",
        "issuetype",
        "status",
        "priority",
        "assignee",
        "reporter",
        "project",
        "parent",
        "labels",
        "created",
        "updated"
    ];

    private static readonly JsonSerializerOptions JsonOptions = new()
    {
        PropertyNameCaseInsensitive = true
    };

    // Projects

    public async Task<List<Project>> GetProjectsAsync(CancellationToken cancellationToken = default)
    {
        var http = httpClientFactory.CreateClient("JiraApi");
        var response = await http.GetAsync("/rest/api/3/project", cancellationToken);
        response.EnsureSuccessStatusCode();

        var dtos = await response.Content.ReadFromJsonAsync<List<JiraProjectDto>>(JsonOptions, cancellationToken);
        return dtos?.Select(p => p.Adapt<Project>()).ToList() ?? [];
    }

    public async Task<Project?> GetProjectAsync(string projectKeyOrId, CancellationToken cancellationToken = default)
    {
        var http = httpClientFactory.CreateClient("JiraApi");
        var response = await http.GetAsync($"/rest/api/3/project/{projectKeyOrId}", cancellationToken);

        if (!response.IsSuccessStatusCode)
        {
            return null;
        }

        var dto = await response.Content.ReadFromJsonAsync<JiraProjectDto>(JsonOptions, cancellationToken);
        return dto?.Adapt<Project>();
    }

    // Issues

    public async Task<Issue?> CreateIssueAsync(string projectKey, string issueType, string summary, string? description, string? parentKey, List<string>? labels, CancellationToken cancellationToken = default)
    {
        var http = httpClientFactory.CreateClient("JiraApi");

        var fields = new Dictionary<string, object?>
        {
            ["project"] = new { key = projectKey },
            ["issuetype"] = new { name = issueType },
            ["summary"] = summary
        };

        if (description is not null)
        {
            fields["description"] = new
            {
                type = "doc",
                version = 1,
                content = new[]
                {
                    new
                    {
                        type = "paragraph",
                        content = new[]
                        {
                            new { type = "text", text = description }
                        }
                    }
                }
            };
        }

        if (parentKey is not null)
        {
            fields["parent"] = new { key = parentKey };
        }

        if (labels is not null && labels.Count > 0)
        {
            fields["labels"] = labels;
        }

        var payload = new { fields };
        var response = await http.PostAsJsonAsync("/rest/api/3/issue", payload, cancellationToken);

        if (!response.IsSuccessStatusCode)
        {
            return null;
        }

        var created = await response.Content.ReadFromJsonAsync<JiraIssueDto>(JsonOptions, cancellationToken);

        if (created?.Key is null)
        {
            return null;
        }

        return await GetIssueAsync(created.Key, cancellationToken);
    }

    public async Task<Issue?> GetIssueAsync(string issueKeyOrId, CancellationToken cancellationToken = default)
    {
        var http = httpClientFactory.CreateClient("JiraApi");
        var response = await http.GetAsync($"/rest/api/3/issue/{issueKeyOrId}", cancellationToken);

        if (!response.IsSuccessStatusCode)
        {
            return null;
        }

        var dto = await response.Content.ReadFromJsonAsync<JiraIssueDto>(JsonOptions, cancellationToken);
        if (dto is null)
        {
            return null;
        }

        var issue = dto.Adapt<Issue>();
        var fieldDefinitions = await GetFieldDefinitionsAsync(cancellationToken);
        PopulateAdditionalFields(issue, dto, fieldDefinitions);

        return issue;
    }

    public async Task<Issue?> UpdateIssueAsync(string issueKeyOrId, string? summary, string? description, CancellationToken cancellationToken = default)
    {
        var http = httpClientFactory.CreateClient("JiraApi");

        var fields = new Dictionary<string, object?>();

        if (summary is not null)
        {
            fields["summary"] = summary;
        }

        if (description is not null)
        {
            fields["description"] = new
            {
                type = "doc",
                version = 1,
                content = new[]
                {
                    new
                    {
                        type = "paragraph",
                        content = new[]
                        {
                            new { type = "text", text = description }
                        }
                    }
                }
            };
        }

        var payload = new { fields };
        var response = await http.PutAsJsonAsync($"/rest/api/3/issue/{issueKeyOrId}", payload, cancellationToken);

        if (!response.IsSuccessStatusCode)
        {
            return null;
        }

        return await GetIssueAsync(issueKeyOrId, cancellationToken);
    }

    public async Task<bool> DeleteIssueAsync(string issueKeyOrId, CancellationToken cancellationToken = default)
    {
        var http = httpClientFactory.CreateClient("JiraApi");
        var response = await http.DeleteAsync($"/rest/api/3/issue/{issueKeyOrId}", cancellationToken);
        return response.IsSuccessStatusCode;
    }

    public async Task<List<Issue>> SearchIssuesAsync(string jql, int maxResults = 50, CancellationToken cancellationToken = default)
    {
        var http = httpClientFactory.CreateClient("JiraApi");
        var payload = new { jql, maxResults, fields = new[] { "summary", "description", "issuetype", "status", "priority", "assignee", "reporter", "project", "parent", "labels", "created", "updated" } };
        var response = await http.PostAsJsonAsync("/rest/api/3/search", payload, cancellationToken);
        response.EnsureSuccessStatusCode();

        var result = await response.Content.ReadFromJsonAsync<JiraSearchResultDto>(JsonOptions, cancellationToken);
        return result?.Issues?.Select(i => i.Adapt<Issue>()).ToList() ?? [];
    }

    private async Task<Dictionary<string, JiraFieldDefinitionDto>> GetFieldDefinitionsAsync(CancellationToken cancellationToken)
    {
        var http = httpClientFactory.CreateClient("JiraApi");
        var response = await http.GetAsync("/rest/api/3/field", cancellationToken);

        if (!response.IsSuccessStatusCode)
        {
            return [];
        }

        var fieldDefinitions = await response.Content.ReadFromJsonAsync<List<JiraFieldDefinitionDto>>(JsonOptions, cancellationToken);
        return fieldDefinitions?
            .Where(f => !string.IsNullOrWhiteSpace(f.Id))
            .ToDictionary(f => f.Id!, StringComparer.OrdinalIgnoreCase) ?? [];
    }

    private static void PopulateAdditionalFields(Issue issue, JiraIssueDto dto, IReadOnlyDictionary<string, JiraFieldDefinitionDto> fieldDefinitions)
    {
        var additionalFields = new List<IssueField>();
        var sections = new Dictionary<string, string?>(StringComparer.OrdinalIgnoreCase);

        foreach (var field in dto.Fields?.AdditionalFields ?? [])
        {
            if (StandardIssueFields.Contains(field.Key) || IsEmpty(field.Value))
            {
                continue;
            }

            fieldDefinitions.TryGetValue(field.Key, out var definition);
            var name = definition?.Name ?? field.Key;
            var plainTextValue = ExtractPlainTextValue(field.Value);

            additionalFields.Add(new IssueField
            {
                Id = field.Key,
                Name = name,
                Type = definition?.Schema?.Type ?? field.Value.ValueKind.ToString(),
                IsCustom = definition?.Custom ?? field.Key.StartsWith("customfield_", StringComparison.OrdinalIgnoreCase),
                PlainTextValue = plainTextValue,
                RawJson = field.Value.GetRawText()
            });

            if (!string.IsNullOrWhiteSpace(plainTextValue))
            {
                sections[name] = plainTextValue;
            }
        }

        issue.AdditionalFields = additionalFields;
        issue.Sections = sections;
    }

    private static string? ExtractPlainTextValue(JsonElement value)
    {
        return value.ValueKind switch
        {
            JsonValueKind.String => value.GetString(),
            JsonValueKind.Object => JiraDocumentParser.ExtractPlainText(value),
            JsonValueKind.Array => JiraDocumentParser.ExtractPlainText(value),
            _ => null
        };
    }

    private static bool IsEmpty(JsonElement value)
    {
        return value.ValueKind switch
        {
            JsonValueKind.Null => true,
            JsonValueKind.Undefined => true,
            JsonValueKind.String => string.IsNullOrWhiteSpace(value.GetString()),
            JsonValueKind.Array => value.GetArrayLength() == 0,
            JsonValueKind.Object => !value.EnumerateObject().Any(),
            _ => false
        };
    }

    // Transitions

    public async Task<List<Transition>> GetTransitionsAsync(string issueKeyOrId, CancellationToken cancellationToken = default)
    {
        var http = httpClientFactory.CreateClient("JiraApi");
        var response = await http.GetAsync($"/rest/api/3/issue/{issueKeyOrId}/transitions", cancellationToken);
        response.EnsureSuccessStatusCode();

        var result = await response.Content.ReadFromJsonAsync<JiraTransitionsResponseDto>(JsonOptions, cancellationToken);
        return result?.Transitions?.Select(t => t.Adapt<Transition>()).ToList() ?? [];
    }

    public async Task<bool> TransitionIssueAsync(string issueKeyOrId, string transitionId, CancellationToken cancellationToken = default)
    {
        var http = httpClientFactory.CreateClient("JiraApi");
        var payload = new { transition = new { id = transitionId } };
        var response = await http.PostAsJsonAsync($"/rest/api/3/issue/{issueKeyOrId}/transitions", payload, cancellationToken);
        return response.IsSuccessStatusCode;
    }

    // Comments

    public async Task<List<Comment>> GetCommentsAsync(string issueKeyOrId, CancellationToken cancellationToken = default)
    {
        var http = httpClientFactory.CreateClient("JiraApi");
        var response = await http.GetAsync($"/rest/api/3/issue/{issueKeyOrId}/comment", cancellationToken);
        response.EnsureSuccessStatusCode();

        var result = await response.Content.ReadFromJsonAsync<JiraCommentsResponseDto>(JsonOptions, cancellationToken);
        return result?.Comments?.Select(c => c.Adapt<Comment>()).ToList() ?? [];
    }

    public async Task<Comment?> AddCommentAsync(string issueKeyOrId, string body, CancellationToken cancellationToken = default)
    {
        var http = httpClientFactory.CreateClient("JiraApi");
        var payload = new
        {
            body = new
            {
                type = "doc",
                version = 1,
                content = new[]
                {
                    new
                    {
                        type = "paragraph",
                        content = new[]
                        {
                            new { type = "text", text = body }
                        }
                    }
                }
            }
        };

        var response = await http.PostAsJsonAsync($"/rest/api/3/issue/{issueKeyOrId}/comment", payload, cancellationToken);

        if (!response.IsSuccessStatusCode)
        {
            return null;
        }

        var dto = await response.Content.ReadFromJsonAsync<JiraCommentDto>(JsonOptions, cancellationToken);
        return dto?.Adapt<Comment>();
    }

    // Labels

    public async Task<bool> AddLabelAsync(string issueKeyOrId, string label, CancellationToken cancellationToken = default)
    {
        var http = httpClientFactory.CreateClient("JiraApi");
        var payload = new
        {
            update = new
            {
                labels = new[] { new { add = label } }
            }
        };
        var response = await http.PutAsJsonAsync($"/rest/api/3/issue/{issueKeyOrId}", payload, cancellationToken);
        return response.IsSuccessStatusCode;
    }

    public async Task<bool> RemoveLabelAsync(string issueKeyOrId, string label, CancellationToken cancellationToken = default)
    {
        var http = httpClientFactory.CreateClient("JiraApi");
        var payload = new
        {
            update = new
            {
                labels = new[] { new { remove = label } }
            }
        };
        var response = await http.PutAsJsonAsync($"/rest/api/3/issue/{issueKeyOrId}", payload, cancellationToken);
        return response.IsSuccessStatusCode;
    }

    // Assignment

    public async Task<bool> AssignIssueAsync(string issueKeyOrId, string? accountId, CancellationToken cancellationToken = default)
    {
        var http = httpClientFactory.CreateClient("JiraApi");
        var payload = new { accountId };
        var response = await http.PutAsJsonAsync($"/rest/api/3/issue/{issueKeyOrId}/assignee", payload, cancellationToken);
        return response.IsSuccessStatusCode;
    }

    // Issue Links

    public async Task<bool> LinkIssuesAsync(string inwardIssueKey, string outwardIssueKey, string linkTypeName, CancellationToken cancellationToken = default)
    {
        var http = httpClientFactory.CreateClient("JiraApi");
        var payload = new
        {
            type = new { name = linkTypeName },
            inwardIssue = new { key = inwardIssueKey },
            outwardIssue = new { key = outwardIssueKey }
        };
        var response = await http.PostAsJsonAsync("/rest/api/3/issueLink", payload, cancellationToken);
        return response.IsSuccessStatusCode;
    }

    // Boards (Agile API)

    public async Task<List<Board>> GetBoardsAsync(CancellationToken cancellationToken = default)
    {
        var http = httpClientFactory.CreateClient("JiraApi");
        var allBoards = new List<Board>();
        var startAt = 0;

        do
        {
            var response = await http.GetAsync($"/rest/agile/1.0/board?startAt={startAt}&maxResults=50", cancellationToken);
            response.EnsureSuccessStatusCode();

            var result = await response.Content.ReadFromJsonAsync<JiraBoardsResponseDto>(JsonOptions, cancellationToken);

            if (result?.Values is not null)
            {
                allBoards.AddRange(result.Values.Select(b => b.Adapt<Board>()));
            }

            if (result is null || result.IsLast)
            {
                break;
            }

            startAt += result.MaxResults;
        } while (true);

        return allBoards;
    }

    public async Task<Board?> GetBoardAsync(int boardId, CancellationToken cancellationToken = default)
    {
        var http = httpClientFactory.CreateClient("JiraApi");
        var response = await http.GetAsync($"/rest/agile/1.0/board/{boardId}", cancellationToken);

        if (!response.IsSuccessStatusCode)
        {
            return null;
        }

        var dto = await response.Content.ReadFromJsonAsync<JiraBoardDto>(JsonOptions, cancellationToken);
        return dto?.Adapt<Board>();
    }

    // Sprints (Agile API)

    public async Task<List<Sprint>> GetSprintsAsync(int boardId, CancellationToken cancellationToken = default)
    {
        var http = httpClientFactory.CreateClient("JiraApi");
        var allSprints = new List<Sprint>();
        var startAt = 0;

        do
        {
            var response = await http.GetAsync($"/rest/agile/1.0/board/{boardId}/sprint?startAt={startAt}&maxResults=50", cancellationToken);
            response.EnsureSuccessStatusCode();

            var result = await response.Content.ReadFromJsonAsync<JiraSprintsResponseDto>(JsonOptions, cancellationToken);

            if (result?.Values is not null)
            {
                allSprints.AddRange(result.Values.Select(s => s.Adapt<Sprint>()));
            }

            if (result is null || result.IsLast)
            {
                break;
            }

            startAt += result.MaxResults;
        } while (true);

        return allSprints;
    }

    public async Task<bool> MoveIssuesToSprintAsync(int sprintId, List<string> issueKeys, CancellationToken cancellationToken = default)
    {
        var http = httpClientFactory.CreateClient("JiraApi");
        var payload = new { issues = issueKeys };
        var response = await http.PostAsJsonAsync($"/rest/agile/1.0/sprint/{sprintId}/issue", payload, cancellationToken);
        return response.IsSuccessStatusCode;
    }
}
