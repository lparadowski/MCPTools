using System.Net.Http.Json;
using System.Text.RegularExpressions;
using System.Text.Json;
using Jira.Application.Interfaces;
using Jira.Domain.Entities;
using Jira.Infrastructure.Dtos;
using Mapster;

namespace Jira.Infrastructure.Clients;

public class JiraClient(IHttpClientFactory httpClientFactory) : IJiraClient
{
    private static readonly JsonSerializerOptions JsonOptions = new()
    {
        PropertyNameCaseInsensitive = true
    };
    private static readonly Regex MarkdownLinkRegex = new(@"\[(?<text>[^\]]+)\]\((?<url>https?://[^\s)]+)\)", RegexOptions.Compiled | RegexOptions.IgnoreCase);
    private static readonly Regex BareUrlRegex = new(@"https?://[^\s]+", RegexOptions.Compiled | RegexOptions.IgnoreCase);

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

    public async Task<Issue?> CreateIssueAsync(
        string projectKey,
        string issueType,
        string summary,
        string? description,
        Dictionary<string, string?>? customFields,
        string? parentKey,
        List<string>? labels,
        CancellationToken cancellationToken = default)
    {
        var http = httpClientFactory.CreateClient("JiraApi");

        var fields = new Dictionary<string, object?>
        {
            ["project"] = new { key = projectKey },
            ["issuetype"] = new { name = issueType },
            ["summary"] = summary
        };

        AddTextDocumentField(fields, "description", description);
        AddCustomFields(fields, customFields);

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
        var response = await http.GetAsync($"/rest/api/3/issue/{issueKeyOrId}?expand=names", cancellationToken);

        if (!response.IsSuccessStatusCode)
        {
            return null;
        }

        var dto = await response.Content.ReadFromJsonAsync<JiraIssueDto>(JsonOptions, cancellationToken);
        return dto is null ? null : MapIssue(dto, dto.Names);
    }

    public async Task<Issue?> UpdateIssueAsync(
        string issueKeyOrId,
        string? summary,
        string? description,
        Dictionary<string, string?>? customFields,
        CancellationToken cancellationToken = default)
    {
        var http = httpClientFactory.CreateClient("JiraApi");

        var fields = new Dictionary<string, object?>();

        if (summary is not null)
        {
            fields["summary"] = summary;
        }

        AddTextDocumentField(fields, "description", description);
        AddCustomFields(fields, customFields);

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
        var fields = new[] { "summary", "description", "issuetype", "status", "priority", "assignee", "reporter", "project", "parent", "labels", "created", "updated" };
        var query = $"jql={Uri.EscapeDataString(jql)}&maxResults={maxResults}";

        foreach (var field in fields)
        {
            query += $"&fields={Uri.EscapeDataString(field)}";
        }

        var response = await http.GetAsync($"/rest/api/3/search/jql?{query}", cancellationToken);
        response.EnsureSuccessStatusCode();

        var result = await response.Content.ReadFromJsonAsync<JiraSearchResultDto>(JsonOptions, cancellationToken);
        return result?.Issues?.Select(i => MapIssue(i, result.Names)).ToList() ?? [];
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

    private static void AddCustomFields(Dictionary<string, object?> fields, Dictionary<string, string?>? customFields)
    {
        if (customFields is null)
        {
            return;
        }

        foreach (var field in customFields)
        {
            AddTextDocumentField(fields, field.Key, field.Value);
        }
    }

    private static void AddTextDocumentField(Dictionary<string, object?> fields, string fieldKey, string? value)
    {
        if (string.IsNullOrWhiteSpace(value))
        {
            return;
        }

        fields[fieldKey] = CreateDocument(value);
    }

    private static object CreateDocument(string text)
    {
        var normalizedText = text.Replace("\r\n", "\n");
        var blocks = normalizedText
            .Split("\n\n", StringSplitOptions.None)
            .SelectMany(CreateBlockNodes)
            .ToArray();

        return new
        {
            type = "doc",
            version = 1,
            content = blocks
        };
    }

    private static IEnumerable<object> CreateBlockNodes(string block)
    {
        var lines = block
            .Split('\n', StringSplitOptions.None)
            .Select(line => line.Trim())
            .Where(line => !string.IsNullOrWhiteSpace(line))
            .ToArray();

        if (lines.Length == 0)
        {
            yield break;
        }

        if (lines.Length == 1 && IsHeading(lines[0]))
        {
            yield return CreateHeading(lines[0]);
            yield break;
        }

        if (lines.All(IsBulletLine))
        {
            yield return CreateBulletList(lines);
            yield break;
        }

        yield return CreateParagraph(string.Join("\n", lines));
    }

    private static bool IsHeading(string line) =>
        line.EndsWith(':') || line.All(character => !char.IsLetter(character) || char.IsUpper(character));

    private static bool IsBulletLine(string line) => line.StartsWith("- ");

    private static object CreateHeading(string line) => new
    {
        type = "heading",
        attrs = new { level = 3 },
        content = CreateInlineContent(line.TrimEnd(':'))
    };

    private static object CreateBulletList(IEnumerable<string> lines) => new
    {
        type = "bulletList",
        content = lines
            .Select(line => new
            {
                type = "listItem",
                content = new object[]
                {
                    CreateParagraph(line[2..].Trim())
                }
            })
            .ToArray()
    };

    private static object CreateParagraph(string text) => new
    {
        type = "paragraph",
        content = CreateInlineContent(text)
    };

    private static object[] CreateInlineContent(string text)
    {
        var nodes = new List<object>();
        var remaining = text;

        while (!string.IsNullOrEmpty(remaining))
        {
            var markdownMatch = MarkdownLinkRegex.Match(remaining);
            var bareUrlMatch = BareUrlRegex.Match(remaining);
            var nextMatch = ChooseNextMatch(markdownMatch, bareUrlMatch);

            if (nextMatch is null)
            {
                nodes.Add(CreateTextNode(remaining));
                break;
            }

            if (nextMatch.Index > 0)
            {
                nodes.Add(CreateTextNode(remaining[..nextMatch.Index]));
            }

            if (nextMatch == markdownMatch)
            {
                nodes.Add(CreateLinkNode(markdownMatch.Groups["text"].Value, markdownMatch.Groups["url"].Value));
            }
            else
            {
                nodes.Add(CreateLinkNode(nextMatch.Value, nextMatch.Value.TrimEnd('.', ',', ';', ')')));
            }

            remaining = remaining[(nextMatch.Index + nextMatch.Length)..];
        }

        return nodes.Count > 0 ? nodes.ToArray() : [CreateTextNode(text)];
    }

    private static Match? ChooseNextMatch(Match markdownMatch, Match bareUrlMatch)
    {
        var hasMarkdown = markdownMatch.Success;
        var hasBareUrl = bareUrlMatch.Success;

        if (!hasMarkdown && !hasBareUrl)
        {
            return null;
        }

        if (!hasMarkdown)
        {
            return bareUrlMatch;
        }

        if (!hasBareUrl)
        {
            return markdownMatch;
        }

        return markdownMatch.Index <= bareUrlMatch.Index ? markdownMatch : bareUrlMatch;
    }

    private static object CreateTextNode(string text) => new
    {
        type = "text",
        text
    };

    private static object CreateLinkNode(string text, string href)
    {
        if (href.Contains("/wiki/", StringComparison.OrdinalIgnoreCase))
        {
            return new
            {
                type = "inlineCard",
                attrs = new
                {
                    url = href
                }
            };
        }

        return new
        {
            type = "text",
            text,
            marks = new object[]
            {
                new
                {
                    type = "link",
                    attrs = new
                    {
                        href
                    }
                }
            }
        };
    }

    private static Issue MapIssue(JiraIssueDto dto, Dictionary<string, string>? fieldNames) => new()
    {
        Id = dto.Id ?? string.Empty,
        Key = dto.Key ?? string.Empty,
        Summary = dto.Fields?.Summary ?? string.Empty,
        Description = ExtractPlainText(dto.Fields?.Description),
        IssueType = dto.Fields?.Issuetype?.Name ?? string.Empty,
        Status = dto.Fields?.Status?.Name ?? string.Empty,
        Priority = dto.Fields?.Priority?.Name,
        AssigneeAccountId = dto.Fields?.Assignee?.AccountId,
        AssigneeDisplayName = dto.Fields?.Assignee?.DisplayName,
        ReporterDisplayName = dto.Fields?.Reporter?.DisplayName,
        ProjectKey = dto.Fields?.Project?.Key,
        ParentKey = dto.Fields?.Parent?.Key,
        Labels = dto.Fields?.Labels ?? [],
        Created = ParseDateTime(dto.Fields?.Created),
        Updated = ParseDateTime(dto.Fields?.Updated),
        Fields = BuildFieldMap(dto.Fields, fieldNames)
    };

    private static DateTime? ParseDateTime(string? value) =>
        value is null ? null : DateTime.Parse(value);

    private static Dictionary<string, object?> BuildFieldMap(JiraIssueFieldsDto? fields, Dictionary<string, string>? fieldNames)
    {
        var result = new Dictionary<string, object?>(StringComparer.OrdinalIgnoreCase);

        if (fields is null)
        {
            return result;
        }

        AddField(result, "summary", fields.Summary, fieldNames);
        AddField(result, "description", ExtractPlainText(fields.Description), fieldNames);
        AddField(result, "issuetype", fields.Issuetype?.Name, fieldNames);
        AddField(result, "status", fields.Status?.Name, fieldNames);
        AddField(result, "priority", fields.Priority?.Name, fieldNames);
        AddField(result, "assignee", fields.Assignee?.DisplayName, fieldNames);
        AddField(result, "reporter", fields.Reporter?.DisplayName, fieldNames);
        AddField(result, "project", fields.Project?.Key, fieldNames);
        AddField(result, "parent", fields.Parent?.Key, fieldNames);
        AddField(result, "labels", fields.Labels, fieldNames);
        AddField(result, "created", fields.Created, fieldNames);
        AddField(result, "updated", fields.Updated, fieldNames);

        if (fields.AdditionalFields is null)
        {
            return result;
        }

        foreach (var field in fields.AdditionalFields)
        {
            AddField(result, field.Key, ConvertFieldValue(field.Value), fieldNames);
        }

        return result;
    }

    private static void AddField(
        IDictionary<string, object?> target,
        string fieldKey,
        object? value,
        IReadOnlyDictionary<string, string>? fieldNames)
    {
        var resolvedKey = ResolveFieldName(fieldKey, fieldNames);

        if (!target.ContainsKey(resolvedKey))
        {
            target[resolvedKey] = value;
            return;
        }

        target[$"{resolvedKey} ({fieldKey})"] = value;
    }

    private static string ResolveFieldName(string fieldKey, IReadOnlyDictionary<string, string>? fieldNames) =>
        fieldNames is not null && fieldNames.TryGetValue(fieldKey, out var fieldName) && !string.IsNullOrWhiteSpace(fieldName)
            ? fieldName
            : fieldKey;

    private static object? ConvertFieldValue(JsonElement value)
    {
        if (LooksLikeAdfDocument(value))
        {
            return ExtractAdfPlainText(value);
        }

        return value.ValueKind switch
        {
            JsonValueKind.Object => ConvertObject(value),
            JsonValueKind.Array => value.EnumerateArray().Select(ConvertFieldValue).ToList(),
            JsonValueKind.String => value.GetString(),
            JsonValueKind.Number when value.TryGetInt64(out var integer) => integer,
            JsonValueKind.Number => value.GetDouble(),
            JsonValueKind.True => true,
            JsonValueKind.False => false,
            JsonValueKind.Null => null,
            _ => value.GetRawText()
        };
    }

    private static object ConvertObject(JsonElement value)
    {
        if (TryGetProperty(value, "displayName", out var displayName))
        {
            return displayName.GetString() ?? string.Empty;
        }

        if (TryGetProperty(value, "name", out var name))
        {
            return name.GetString() ?? string.Empty;
        }

        if (TryGetProperty(value, "key", out var key))
        {
            return key.GetString() ?? string.Empty;
        }

        var dictionary = new Dictionary<string, object?>(StringComparer.OrdinalIgnoreCase);

        foreach (var property in value.EnumerateObject())
        {
            dictionary[property.Name] = ConvertFieldValue(property.Value);
        }

        return dictionary;
    }

    private static bool LooksLikeAdfDocument(JsonElement value) =>
        value.ValueKind == JsonValueKind.Object
        && TryGetProperty(value, "type", out var typeProperty)
        && typeProperty.ValueKind == JsonValueKind.String
        && string.Equals(typeProperty.GetString(), "doc", StringComparison.OrdinalIgnoreCase)
        && TryGetProperty(value, "content", out _);

    private static bool TryGetProperty(JsonElement value, string propertyName, out JsonElement property)
    {
        foreach (var candidate in value.EnumerateObject())
        {
            if (string.Equals(candidate.Name, propertyName, StringComparison.OrdinalIgnoreCase))
            {
                property = candidate.Value;
                return true;
            }
        }

        property = default;
        return false;
    }

    private static string? ExtractPlainText(JiraDocumentDto? doc)
    {
        if (doc?.Content is null)
        {
            return null;
        }

        var texts = doc.Content
            .Where(block => block.Content is not null)
            .SelectMany(block => block.Content!)
            .Select(inline => inline.Text)
            .Where(text => !string.IsNullOrWhiteSpace(text))
            .ToList();

        return texts.Count > 0 ? string.Join(" ", texts) : null;
    }

    private static string? ExtractAdfPlainText(JsonElement document)
    {
        var blocks = ExtractAdfBlocks(document)
            .Where(text => !string.IsNullOrWhiteSpace(text))
            .ToList();

        return blocks.Count > 0 ? string.Join("\n\n", blocks) : null;
    }

    private static IEnumerable<string> ExtractAdfBlocks(JsonElement node)
    {
        if (!TryGetProperty(node, "content", out var content) || content.ValueKind != JsonValueKind.Array)
        {
            yield break;
        }

        foreach (var child in content.EnumerateArray())
        {
            var text = ExtractAdfNodeText(child);

            if (!string.IsNullOrWhiteSpace(text))
            {
                yield return text;
            }
        }
    }

    private static string? ExtractAdfNodeText(JsonElement node)
    {
        var nodeType = TryGetProperty(node, "type", out var typeProperty) ? typeProperty.GetString() : null;

        return nodeType switch
        {
            "text" => TryGetProperty(node, "text", out var textProperty) ? textProperty.GetString() : null,
            "paragraph" => ExtractAdfInlineText(node),
            "heading" => ExtractAdfInlineText(node),
            "inlineCard" => TryGetProperty(node, "attrs", out var attrs) && TryGetProperty(attrs, "url", out var url) ? url.GetString() : null,
            "bulletList" => ExtractAdfListText(node, ordered: false),
            "orderedList" => ExtractAdfListText(node, ordered: true),
            "listItem" => ExtractAdfListItemText(node),
            _ => ExtractAdfInlineText(node)
        };
    }

    private static string? ExtractAdfInlineText(JsonElement node)
    {
        if (!TryGetProperty(node, "content", out var content) || content.ValueKind != JsonValueKind.Array)
        {
            return null;
        }

        var parts = content.EnumerateArray()
            .Select(ExtractAdfNodeText)
            .Where(text => !string.IsNullOrWhiteSpace(text))
            .ToList();

        return parts.Count > 0 ? string.Join(" ", parts) : null;
    }

    private static string? ExtractAdfListText(JsonElement node, bool ordered)
    {
        if (!TryGetProperty(node, "content", out var content) || content.ValueKind != JsonValueKind.Array)
        {
            return null;
        }

        var items = content.EnumerateArray().ToList();

        var lines = items
            .Select((item, index) => ExtractAdfListItemText(item, ordered ? $"{index + 1}. " : "- "))
            .Where(text => !string.IsNullOrWhiteSpace(text))
            .ToList();

        return lines.Count > 0 ? string.Join("\n", lines) : null;
    }

    private static string? ExtractAdfListItemText(JsonElement node) =>
        ExtractAdfListItemText(node, string.Empty);

    private static string? ExtractAdfListItemText(JsonElement node, string prefix)
    {
        if (!TryGetProperty(node, "content", out var content) || content.ValueKind != JsonValueKind.Array)
        {
            return null;
        }

        var parts = content.EnumerateArray()
            .Select(ExtractAdfNodeText)
            .Where(text => !string.IsNullOrWhiteSpace(text))
            .ToList();

        return parts.Count > 0 ? prefix + string.Join(" ", parts) : null;
    }
}
