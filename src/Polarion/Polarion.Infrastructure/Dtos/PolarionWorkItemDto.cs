using System.Text.Json.Serialization;

namespace Polarion.Infrastructure.Dtos;

public class PolarionWorkItemDto
{
    [JsonPropertyName("type")]
    public string? Type { get; set; }

    [JsonPropertyName("id")]
    public string? Id { get; set; }

    [JsonPropertyName("attributes")]
    public PolarionWorkItemAttributesDto? Attributes { get; set; }

    [JsonPropertyName("relationships")]
    public PolarionWorkItemRelationshipsDto? Relationships { get; set; }
}
