using System.Text.Json.Serialization;

namespace Polarion.Infrastructure.Dtos;

public class PolarionLinkedWorkItemDto
{
    [JsonPropertyName("type")]
    public string? Type { get; set; }

    [JsonPropertyName("id")]
    public string? Id { get; set; }

    [JsonPropertyName("attributes")]
    public PolarionLinkedWorkItemAttributesDto? Attributes { get; set; }
}
