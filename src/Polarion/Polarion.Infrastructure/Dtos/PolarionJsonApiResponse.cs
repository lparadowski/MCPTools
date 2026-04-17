using System.Text.Json.Serialization;

namespace Polarion.Infrastructure.Dtos;

public class PolarionJsonApiResponse<T>
{
    [JsonPropertyName("data")]
    public List<T>? Data { get; set; }

    [JsonPropertyName("links")]
    public PolarionLinksDto? Links { get; set; }

    [JsonPropertyName("meta")]
    public PolarionMetaDto? Meta { get; set; }
}
