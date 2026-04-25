using System.Text.Json.Serialization;

namespace Miro.Infrastructure.Dtos;

internal class MiroItemStyleDto
{
    [JsonPropertyName("fillColor")]
    public string? FillColor { get; set; }
}
