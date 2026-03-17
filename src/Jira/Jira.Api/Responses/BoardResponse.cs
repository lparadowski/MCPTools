namespace Jira.Api.Responses;

public class BoardResponse
{
    public int Id { get; set; }
    public required string Name { get; set; }
    public string? Type { get; set; }
    public string? ProjectKey { get; set; }
}
