using System.Text.Json.Serialization;

namespace Polarion.Infrastructure.Dtos;

public class PolarionRelationshipDataDto
{
    [JsonPropertyName("type")]
    public string? Type { get; set; }

    [JsonPropertyName("id")]
    public string? Id { get; set; }
}
