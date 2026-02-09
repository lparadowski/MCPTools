using System.Text.Json.Serialization;

namespace Miro.Infrastructure.Dtos;

internal class MiroItemDataDto
{
    [JsonPropertyName("content")]
    public string? Content { get; set; }

    [JsonPropertyName("shape")]
    public string? Shape { get; set; }
}
