using System.Text.Json.Serialization;

namespace Polarion.Infrastructure.Dtos;

public class PolarionWorkItemRelationshipsDto
{
    [JsonPropertyName("author")]
    public PolarionRelationshipDto? Author { get; set; }

    [JsonPropertyName("linkedWorkItems")]
    public PolarionRelationshipDto? LinkedWorkItems { get; set; }
}
