namespace Jira.Api.Responses;

public class ChunkedContentResponse
{
    public required string Content { get; set; }
    public int TotalLength { get; set; }
    public bool HasMore { get; set; }
    public int? NextOffset { get; set; }
}
