namespace Jira.Api.Responses;

public class TransitionResponse
{
    public required string Id { get; set; }
    public required string Name { get; set; }
    public string? ToStatus { get; set; }
}
