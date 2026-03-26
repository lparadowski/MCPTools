namespace Shared.Application.Chunking;

public static class ContentChunker
{
    public const int DefaultMaxLength = 20000;

    public static ChunkedContent Chunk(string content, int offset = 0, int maxLength = DefaultMaxLength)
    {
        if (string.IsNullOrEmpty(content))
        {
            return new ChunkedContent
            {
                Content = string.Empty,
                TotalLength = 0,
                HasMore = false,
                NextOffset = null
            };
        }

        var totalLength = content.Length;

        if (offset >= totalLength)
        {
            return new ChunkedContent
            {
                Content = string.Empty,
                TotalLength = totalLength,
                HasMore = false,
                NextOffset = null
            };
        }

        var remainingLength = totalLength - offset;
        var chunkLength = Math.Min(remainingLength, maxLength);
        var chunk = content.Substring(offset, chunkLength);
        var newOffset = offset + chunkLength;
        var hasMore = newOffset < totalLength;

        return new ChunkedContent
        {
            Content = chunk,
            TotalLength = totalLength,
            HasMore = hasMore,
            NextOffset = hasMore ? newOffset : null
        };
    }
}
