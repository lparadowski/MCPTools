namespace GitHub.Domain.Entities;

public class Repository
{
    public required string Id { get; set; }
    public required string Name { get; set; }
    public required string FullName { get; set; }
    public string? Description { get; set; }
    public string? Url { get; set; }
    public bool Private { get; set; }
    public string? DefaultBranch { get; set; }
    public string? Language { get; set; }
    public DateTime? CreatedAt { get; set; }
    public DateTime? UpdatedAt { get; set; }
}
