using System.Text.Json;
using System.Text.Json.Serialization;

namespace Jira.Infrastructure.Dtos;

public class JiraIssueFieldsDto
{
    public string? Summary { get; set; }
    public JiraDocumentDto? Description { get; set; }
    public JiraIssueTypeDto? Issuetype { get; set; }
    public JiraStatusDto? Status { get; set; }
    public JiraPriorityDto? Priority { get; set; }
    public JiraUserDto? Assignee { get; set; }
    public JiraUserDto? Reporter { get; set; }
    public JiraProjectDto? Project { get; set; }
    public JiraParentDto? Parent { get; set; }
    public List<string>? Labels { get; set; }
    public string? Created { get; set; }
    public string? Updated { get; set; }

    [JsonExtensionData]
    public Dictionary<string, JsonElement>? AdditionalFields { get; set; }
}
