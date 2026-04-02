namespace AzureDevOps.Api.Responses;

public class WorkItemResponse
{
    public required int Id { get; set; }
    public required string Title { get; set; }
    public string? Description { get; set; }
    public required string WorkItemType { get; set; }
    public required string State { get; set; }
    public string? AssignedTo { get; set; }
    public string? AreaPath { get; set; }
    public string? IterationPath { get; set; }
    public string? Priority { get; set; }
    public string? Tags { get; set; }
    public int? ParentId { get; set; }
    public string? ProjectName { get; set; }
    public DateTime? CreatedDate { get; set; }
    public DateTime? ChangedDate { get; set; }
    public string? Url { get; set; }
}
