namespace AzureDevOps.Api.Responses;

public class CommentResponse
{
    public required int Id { get; set; }
    public required string Text { get; set; }
    public string? CreatedBy { get; set; }
    public DateTime? CreatedDate { get; set; }
}
