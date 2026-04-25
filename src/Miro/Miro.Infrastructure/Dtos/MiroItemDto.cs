using System.Text.Json.Serialization;

namespace Miro.Infrastructure.Dtos;

internal class MiroItemDto
{
    [JsonPropertyName("id")]
    public string Id { get; set; } = string.Empty;

    [JsonPropertyName("type")]
    public string Type { get; set; } = string.Empty;

    [JsonPropertyName("data")]
    public MiroItemDataDto? Data { get; set; }

    [JsonPropertyName("style")]
    public MiroItemStyleDto? Style { get; set; }

    [JsonPropertyName("position")]
    public MiroItemPositionDto? Position { get; set; }

    [JsonPropertyName("createdAt")]
    public DateTime? CreatedAt { get; set; }

    [JsonPropertyName("modifiedAt")]
    public DateTime? ModifiedAt { get; set; }
}
