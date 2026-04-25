namespace Shared.Application.Chunking;

public class ChunkedContent
{
    public required string Content { get; set; }
    public int TotalLength { get; set; }
    public bool HasMore { get; set; }
    public int? NextOffset { get; set; }
}
