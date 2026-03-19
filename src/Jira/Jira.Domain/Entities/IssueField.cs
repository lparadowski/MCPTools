namespace Jira.Domain.Entities;

public class IssueField
{
    public required string Id { get; set; }
    public required string Name { get; set; }
    public string? Type { get; set; }
    public bool IsCustom { get; set; }
    public string? PlainTextValue { get; set; }
    public string? RawJson { get; set; }
}
