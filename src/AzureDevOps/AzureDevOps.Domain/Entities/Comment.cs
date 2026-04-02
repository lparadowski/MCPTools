namespace AzureDevOps.Domain.Entities;

public class Comment
{
    public required int Id { get; set; }
    public required string Text { get; set; }
    public string? CreatedBy { get; set; }
    public DateTime? CreatedDate { get; set; }
}
