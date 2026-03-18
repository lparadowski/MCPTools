namespace Jira.Api.Responses;

public class ProjectResponse
{
    public required string Id { get; set; }
    public required string Key { get; set; }
    public required string Name { get; set; }
    public string? Description { get; set; }
    public string? ProjectTypeKey { get; set; }
    public string? Style { get; set; }
    public string? LeadAccountId { get; set; }
    public string? LeadDisplayName { get; set; }
    public string? Url { get; set; }
}
