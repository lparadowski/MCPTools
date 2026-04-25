using System.Text.Json.Serialization;

namespace Polarion.Infrastructure.Dtos;

public class PolarionLinksDto
{
    [JsonPropertyName("self")]
    public string? Self { get; set; }

    [JsonPropertyName("first")]
    public string? First { get; set; }

    [JsonPropertyName("next")]
    public string? Next { get; set; }

    [JsonPropertyName("last")]
    public string? Last { get; set; }
}
