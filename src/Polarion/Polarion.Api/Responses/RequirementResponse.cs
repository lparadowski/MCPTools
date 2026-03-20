namespace Polarion.Api.Responses;

public class RequirementResponse
{
    public required string Id { get; set; }
    public required string Title { get; set; }
    public string? Description { get; set; }
    public string? Type { get; set; }
    public string? Status { get; set; }
    public string? Priority { get; set; }
    public string? Severity { get; set; }
    public string? AuthorId { get; set; }
    public string? ProjectId { get; set; }
    public DateTime? Created { get; set; }
    public DateTime? Updated { get; set; }
}
