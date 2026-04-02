using System.Text.Json.Serialization;

namespace AzureDevOps.Infrastructure.Dtos;

public class SprintDto
{
    [JsonPropertyName("id")]
    public string? Id { get; set; }

    [JsonPropertyName("name")]
    public string? Name { get; set; }

    [JsonPropertyName("path")]
    public string? Path { get; set; }

    [JsonPropertyName("attributes")]
    public SprintAttributesDto? Attributes { get; set; }
}
