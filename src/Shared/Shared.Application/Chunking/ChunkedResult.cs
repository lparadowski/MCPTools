namespace Shared.Application.Chunking;

public class ChunkedResult<T>
{
    public required T Value { get; set; }
    public ChunkedContent? ChunkMetadata { get; set; }
}
