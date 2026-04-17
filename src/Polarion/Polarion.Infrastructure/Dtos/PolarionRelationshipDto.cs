using System.Text.Json.Serialization;

namespace Polarion.Infrastructure.Dtos;

public class PolarionRelationshipDto
{
    [JsonPropertyName("data")]
    public PolarionRelationshipDataDto? Data { get; set; }
}
