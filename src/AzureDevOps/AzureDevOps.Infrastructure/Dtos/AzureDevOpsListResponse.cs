using System.Text.Json.Serialization;

namespace AzureDevOps.Infrastructure.Dtos;

public class AzureDevOpsListResponse<T>
{
    [JsonPropertyName("count")]
    public int Count { get; set; }

    [JsonPropertyName("value")]
    public List<T> Value { get; set; } = [];
}
