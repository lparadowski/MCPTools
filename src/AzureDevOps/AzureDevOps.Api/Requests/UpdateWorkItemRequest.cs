namespace AzureDevOps.Api.Requests;

public class UpdateWorkItemRequest
{
    public string? Title { get; set; }
    public string? Description { get; set; }
    public string? State { get; set; }
    public string? AssignedTo { get; set; }
    public string? AreaPath { get; set; }
    public string? IterationPath { get; set; }
    public string? Priority { get; set; }
    public string? Tags { get; set; }
}
