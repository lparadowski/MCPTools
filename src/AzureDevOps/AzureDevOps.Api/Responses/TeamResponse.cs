namespace AzureDevOps.Api.Responses;

public class TeamResponse
{
    public required string Id { get; set; }
    public required string Name { get; set; }
    public string? Description { get; set; }
    public string? ProjectName { get; set; }
}
