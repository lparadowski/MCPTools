using System.Text.Json.Serialization;

namespace AzureDevOps.Infrastructure.Dtos;

public class WiqlWorkItemRefDto
{
    [JsonPropertyName("id")]
    public int Id { get; set; }

    [JsonPropertyName("url")]
    public string? Url { get; set; }
}
