using System.Text.Json.Serialization;

namespace Miro.Infrastructure.Dtos;

internal class MiroItemsPageDto
{
    [JsonPropertyName("data")]
    public List<MiroItemDto>? Data { get; set; }

    [JsonPropertyName("cursor")]
    public string? Cursor { get; set; }

    [JsonPropertyName("limit")]
    public int Limit { get; set; }

    [JsonPropertyName("size")]
    public int Size { get; set; }

    [JsonPropertyName("total")]
    public int Total { get; set; }
}
