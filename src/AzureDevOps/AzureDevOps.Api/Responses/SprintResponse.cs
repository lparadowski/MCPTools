namespace AzureDevOps.Api.Responses;

public class SprintResponse
{
    public required string Id { get; set; }
    public required string Name { get; set; }
    public string? Path { get; set; }
    public DateTime? StartDate { get; set; }
    public DateTime? FinishDate { get; set; }
    public string? TimeFrame { get; set; }
}
