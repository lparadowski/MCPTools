using System.Text.Json.Serialization;

namespace Polarion.Infrastructure.Dtos;

public class PolarionHtmlContentDto
{
    [JsonPropertyName("type")]
    public string? Type { get; set; }

    [JsonPropertyName("value")]
    public string? Value { get; set; }
}
