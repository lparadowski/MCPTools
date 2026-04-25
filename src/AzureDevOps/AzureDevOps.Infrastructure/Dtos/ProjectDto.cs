using System.Text.Json.Serialization;

namespace AzureDevOps.Infrastructure.Dtos;

public class ProjectDto
{
    [JsonPropertyName("id")]
    public string? Id { get; set; }

    [JsonPropertyName("name")]
    public string? Name { get; set; }

    [JsonPropertyName("description")]
    public string? Description { get; set; }

    [JsonPropertyName("state")]
    public string? State { get; set; }

    [JsonPropertyName("url")]
    public string? Url { get; set; }
}
