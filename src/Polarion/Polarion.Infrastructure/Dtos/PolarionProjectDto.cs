using System.Text.Json.Serialization;

namespace Polarion.Infrastructure.Dtos;

public class PolarionProjectDto
{
    [JsonPropertyName("type")]
    public string? Type { get; set; }

    [JsonPropertyName("id")]
    public string? Id { get; set; }

    [JsonPropertyName("attributes")]
    public PolarionProjectAttributesDto? Attributes { get; set; }
}
