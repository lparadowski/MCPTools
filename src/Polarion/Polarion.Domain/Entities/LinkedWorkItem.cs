namespace Polarion.Domain.Entities;

public class LinkedWorkItem
{
    public required string WorkItemId { get; set; }
    public string? Role { get; set; }
    public string? LinkType { get; set; }
}
