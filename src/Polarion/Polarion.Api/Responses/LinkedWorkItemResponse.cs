namespace Polarion.Api.Responses;

public class LinkedWorkItemResponse
{
    public required string WorkItemId { get; set; }
    public string? Role { get; set; }
    public string? LinkType { get; set; }
}
