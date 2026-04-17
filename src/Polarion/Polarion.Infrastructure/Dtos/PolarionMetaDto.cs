using System.Text.Json.Serialization;

namespace Polarion.Infrastructure.Dtos;

public class PolarionMetaDto
{
    [JsonPropertyName("totalCount")]
    public int? TotalCount { get; set; }
}
