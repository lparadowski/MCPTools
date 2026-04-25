using System.Text.Json.Serialization;

namespace Polarion.Infrastructure.Dtos;

public class PolarionLinkedWorkItemAttributesDto
{
    [JsonPropertyName("role")]
    public string? Role { get; set; }

    [JsonPropertyName("suspect")]
    public bool? Suspect { get; set; }
}
