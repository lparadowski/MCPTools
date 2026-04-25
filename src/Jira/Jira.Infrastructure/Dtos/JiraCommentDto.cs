using System.Text.Json;

namespace Jira.Infrastructure.Dtos;

public class JiraCommentDto
{
    public string? Id { get; set; }
    public JiraUserDto? Author { get; set; }
    public JsonElement? Body { get; set; }
    public string? Created { get; set; }
    public string? Updated { get; set; }
}
