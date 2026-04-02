namespace AzureDevOps.Api.Requests;

public class CreateWorkItemRequest
{
    public required string Project { get; set; }
    public required string WorkItemType { get; set; }
    public required string Title { get; set; }
    public string? Description { get; set; }
    public int? ParentId { get; set; }
    public string? AssignedTo { get; set; }
    public string? AreaPath { get; set; }
    public string? IterationPath { get; set; }
    public string? Priority { get; set; }
    public string? Tags { get; set; }
}
