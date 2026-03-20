using System.Text.Json.Serialization;

namespace Polarion.Infrastructure.Dtos;

public class PolarionProjectAttributesDto
{
    [JsonPropertyName("name")]
    public string? Name { get; set; }

    [JsonPropertyName("description")]
    public PolarionHtmlContentDto? Description { get; set; }

    [JsonPropertyName("trackerPrefix")]
    public string? TrackerPrefix { get; set; }
}
