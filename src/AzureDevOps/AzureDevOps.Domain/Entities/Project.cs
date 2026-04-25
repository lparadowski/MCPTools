namespace AzureDevOps.Domain.Entities;

public class Project
{
    public required string Id { get; set; }
    public required string Name { get; set; }
    public string? Description { get; set; }
    public required string State { get; set; }
    public string? Url { get; set; }
}
